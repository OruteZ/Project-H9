using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public static class MeshCut
{

    private static Plane _blade;
    private static Transform _victimTransform;
    private static Mesh _victimMesh;

    private static bool[] _sides = new bool[3];

    // Gathering values
    private static readonly List<int>[] LEFT_GATHER_SUB_INDICES = new List<int>[] { new List<int>(), new List<int>() };
    private static readonly List<int>[] RIGHT_GATHER_SUB_INDICES = new List<int>[] { new List<int>(), new List<int>() };

    private static readonly List<Vector3>[] LEFT_GATHER_ADDED_POINTS = new List<Vector3>[] { new List<Vector3>(), new List<Vector3>() };
    private static readonly List<Vector2>[] LEFT_GATHER_ADDED_UVS = new List<Vector2>[] { new List<Vector2>(), new List<Vector2>() };
    private static readonly List<Vector3>[] LEFT_GATHER_ADDED_NORMALS = new List<Vector3>[] { new List<Vector3>(), new List<Vector3>() };

    private static readonly List<Vector3>[] RIGHT_GATHER_ADDED_POINTS = new List<Vector3>[] { new List<Vector3>(), new List<Vector3>() };
    private static readonly List<Vector2>[] RIGHT_GATHER_ADDED_UVS = new List<Vector2>[] { new List<Vector2>(), new List<Vector2>() };
    private static readonly List<Vector3>[] RIGHT_GATHER_ADDED_NORMALS = new List<Vector3>[] { new List<Vector3>(), new List<Vector3>() };



    // face cutting temps
    private static Vector3 _leftPoint1 = Vector3.zero;
    private static Vector3 _leftPoint2 = Vector3.zero;
    private static Vector3 _rightPoint1 = Vector3.zero;
    private static Vector3 _rightPoint2 = Vector3.zero;

    private static Vector2 _leftUV1 = Vector3.zero;
    private static Vector2 _leftUV2 = Vector3.zero;
    private static Vector2 _rightUV1 = Vector3.zero;
    private static Vector2 _rightUV2 = Vector3.zero;

    private static Vector3 _leftNormal1 = Vector3.zero;
    private static Vector3 _leftNormal2 = Vector3.zero;
    private static Vector3 _rightNormal1 = Vector3.zero;
    private static Vector3 _rightNormal2 = Vector3.zero;


    // final arrays
    private static readonly List<int>[] LEFT_FINAL_SUB_INDICES = new List<int>[] { new List<int>(), new List<int>() };

    private static readonly List<Vector3> LEFT_FINAL_VERTICES = new List<Vector3>();
    private static readonly List<Vector3> LEFT_FINAL_NORMALS = new List<Vector3>();
    private static readonly List<Vector2> LEFT_FINAL_UVS = new List<Vector2>();

    private static readonly List<int>[] RIGHT_FINAL_SUB_INDICES = new List<int>[] { new List<int>(), new List<int>() };

    private static readonly List<Vector3> RIGHT_FINAL_VERTICES = new List<Vector3>();
    private static readonly List<Vector3> RIGHT_FINAL_NORMALS = new List<Vector3>();
    private static readonly List<Vector2> RIGHT_FINAL_UVS = new List<Vector2>();

    // capping stuff
    private static readonly List<Vector3> CREATED_VERTEX_POINTS = new List<Vector3>();

    public static GameObject[] Cut(GameObject victim, Vector3 anchorPoint, Vector3 normalDirection, Material capMaterial)
    {

        _victimTransform = victim.transform;

        _blade = new Plane(victim.transform.InverseTransformDirection(-normalDirection),
                          victim.transform.InverseTransformPoint(anchorPoint));

        _victimMesh = victim.GetComponent<MeshFilter>().mesh;
        _victimMesh.subMeshCount = 2;

        ResetGatheringValues();


        int p1 = 0;
        int p2 = 0;
        int p3 = 0;

        _sides = new bool[3];

        int sub = 0;
        int[] indices = _victimMesh.triangles;
        int[] secondIndices = _victimMesh.GetIndices(1);

        for (int i = 0; i < indices.Length; i += 3)
        {

            p1 = indices[i];
            p2 = indices[i + 1];
            p3 = indices[i + 2];

            _sides[0] = _blade.GetSide(_victimMesh.vertices[p1]);
            _sides[1] = _blade.GetSide(_victimMesh.vertices[p2]);
            _sides[2] = _blade.GetSide(_victimMesh.vertices[p3]);


            sub = 0;
            for (int k = 0; k < secondIndices.Length; k++)
            {
                if (secondIndices[k] == p1)
                {
                    sub = 1;
                    break;
                }
            }


            if (_sides[0] == _sides[1] && _sides[0] == _sides[2])
            { // whole face

                if (_sides[0])
                { // left side
                    LEFT_GATHER_SUB_INDICES[sub].Add(p1);
                    LEFT_GATHER_SUB_INDICES[sub].Add(p2);
                    LEFT_GATHER_SUB_INDICES[sub].Add(p3);

                }
                else
                {

                    RIGHT_GATHER_SUB_INDICES[sub].Add(p1);
                    RIGHT_GATHER_SUB_INDICES[sub].Add(p2);
                    RIGHT_GATHER_SUB_INDICES[sub].Add(p3);

                }

            }
            else
            { // cut face
                ResetFaceCuttingTemps();
                Cut_this_Face(sub, p1, p2, p3);
            }
        }


        // set final arrays
        ResetFinalArrays();
        SetFinalArrays_withOriginals();
        AddNewTriangles_toFinalArrays();
        MakeCaps();

        Mesh leftHalfMesh = new Mesh
        {
            name = "Split Mesh Left",
            vertices = LEFT_FINAL_VERTICES.ToArray(),
            subMeshCount = 2
        };

        leftHalfMesh.SetIndices(LEFT_FINAL_SUB_INDICES[0].ToArray(), MeshTopology.Triangles, 0);
        leftHalfMesh.SetIndices(LEFT_FINAL_SUB_INDICES[1].ToArray(), MeshTopology.Triangles, 1);

        leftHalfMesh.normals = LEFT_FINAL_NORMALS.ToArray();
        leftHalfMesh.uv = LEFT_FINAL_UVS.ToArray();


        Mesh rightHalfMesh = new Mesh
        {
            name = "Split Mesh Right",
            vertices = RIGHT_FINAL_VERTICES.ToArray(),
            subMeshCount = 2
        };

        rightHalfMesh.SetIndices(RIGHT_FINAL_SUB_INDICES[0].ToArray(), MeshTopology.Triangles, 0);
        rightHalfMesh.SetIndices(RIGHT_FINAL_SUB_INDICES[1].ToArray(), MeshTopology.Triangles, 1);

        rightHalfMesh.normals = RIGHT_FINAL_NORMALS.ToArray();
        rightHalfMesh.uv = RIGHT_FINAL_UVS.ToArray();

        victim.name = "leftSide";
        victim.GetComponent<MeshFilter>().mesh = leftHalfMesh;

        Material[] mats = new Material[] { victim.GetComponent<MeshRenderer>().material, capMaterial };

        GameObject leftSideObj = victim;

        GameObject rightSideObj = new GameObject("rightSide", typeof(MeshFilter), typeof(MeshRenderer));
        rightSideObj.transform.position = _victimTransform.position;
        rightSideObj.transform.rotation = _victimTransform.rotation;
        rightSideObj.GetComponent<MeshFilter>().mesh = rightHalfMesh;


        leftSideObj.GetComponent<MeshRenderer>().materials = mats;
        rightSideObj.GetComponent<MeshRenderer>().materials = mats;

        if (leftHalfMesh.vertices.Length is 0)
        {
            Object.Destroy(leftSideObj);
            return new[] { rightSideObj };
        }

        if (rightHalfMesh.vertices.Length is 0)
        {
            Object.Destroy(rightSideObj);
            return new[] { leftSideObj };
        }
        
        return new GameObject[] { leftSideObj, rightSideObj };

    }

    static void ResetGatheringValues()
    {

        LEFT_GATHER_SUB_INDICES[0].Clear();
        LEFT_GATHER_SUB_INDICES[1].Clear();
        LEFT_GATHER_ADDED_POINTS[0].Clear();
        LEFT_GATHER_ADDED_POINTS[1].Clear();
        LEFT_GATHER_ADDED_UVS[0].Clear();
        LEFT_GATHER_ADDED_UVS[1].Clear();
        LEFT_GATHER_ADDED_NORMALS[0].Clear();
        LEFT_GATHER_ADDED_NORMALS[1].Clear();

        RIGHT_GATHER_SUB_INDICES[0].Clear();
        RIGHT_GATHER_SUB_INDICES[1].Clear();
        RIGHT_GATHER_ADDED_POINTS[0].Clear();
        RIGHT_GATHER_ADDED_POINTS[1].Clear();
        RIGHT_GATHER_ADDED_UVS[0].Clear();
        RIGHT_GATHER_ADDED_UVS[1].Clear();
        RIGHT_GATHER_ADDED_NORMALS[0].Clear();
        RIGHT_GATHER_ADDED_NORMALS[1].Clear();

        CREATED_VERTEX_POINTS.Clear();

    }

    static void ResetFaceCuttingTemps()
    {

        _leftPoint1 = Vector3.zero;
        _leftPoint2 = Vector3.zero;
        _rightPoint1 = Vector3.zero;
        _rightPoint2 = Vector3.zero;

        _leftUV1 = Vector3.zero;
        _leftUV2 = Vector3.zero;
        _rightUV1 = Vector3.zero;
        _rightUV2 = Vector3.zero;

        _leftNormal1 = Vector3.zero;
        _leftNormal2 = Vector3.zero;
        _rightNormal1 = Vector3.zero;
        _rightNormal2 = Vector3.zero;

    }

    static void Cut_this_Face(int submesh, int index1, int index2, int index3)
    {

        int p = index1;
        for (int side = 0; side < 3; side++)
        {

            switch (side)
            {
                case 0:
                    p = index1;
                    break;
                case 1:
                    p = index2;
                    break;
                case 2:
                    p = index3;
                    break;

            }

            if (_sides[side])
            {
                if (_leftPoint1 == Vector3.zero)
                {

                    _leftPoint1 = _victimMesh.vertices[p];
                    _leftPoint2 = _leftPoint1;
                    _leftUV1 = _victimMesh.uv[p];
                    _leftUV2 = _leftUV1;
                    _leftNormal1 = _victimMesh.normals[p];
                    _leftNormal2 = _leftNormal1;

                }
                else
                {

                    _leftPoint2 = _victimMesh.vertices[p];
                    _leftUV2 = _victimMesh.uv[p];
                    _leftNormal2 = _victimMesh.normals[p];

                }
            }
            else
            {
                if (_rightPoint1 == Vector3.zero)
                {

                    _rightPoint1 = _victimMesh.vertices[p];
                    _rightPoint2 = _rightPoint1;
                    _rightUV1 = _victimMesh.uv[p];
                    _rightUV2 = _rightUV1;
                    _rightNormal1 = _victimMesh.normals[p];
                    _rightNormal2 = _rightNormal1;

                }
                else
                {

                    _rightPoint2 = _victimMesh.vertices[p];
                    _rightUV2 = _victimMesh.uv[p];
                    _rightNormal2 = _victimMesh.normals[p];

                }
            }
        }


        float normalizedDistance = 0.0f;
        float distance = 0;
        _blade.Raycast(new Ray(_leftPoint1, (_rightPoint1 - _leftPoint1).normalized), out distance);

        normalizedDistance = distance / (_rightPoint1 - _leftPoint1).magnitude;
        Vector3 newVertex1 = Vector3.Lerp(_leftPoint1, _rightPoint1, normalizedDistance);
        Vector2 newUv1 = Vector2.Lerp(_leftUV1, _rightUV1, normalizedDistance);
        Vector3 newNormal1 = Vector3.Lerp(_leftNormal1, _rightNormal1, normalizedDistance);

        CREATED_VERTEX_POINTS.Add(newVertex1);

        _blade.Raycast(new Ray(_leftPoint2, (_rightPoint2 - _leftPoint2).normalized), out distance);

        normalizedDistance = distance / (_rightPoint2 - _leftPoint2).magnitude;
        Vector3 newVertex2 = Vector3.Lerp(_leftPoint2, _rightPoint2, normalizedDistance);
        Vector2 newUv2 = Vector2.Lerp(_leftUV2, _rightUV2, normalizedDistance);
        Vector3 newNormal2 = Vector3.Lerp(_leftNormal2, _rightNormal2, normalizedDistance);

        CREATED_VERTEX_POINTS.Add(newVertex2);

        // first triangle
        Add_Left_triangle(submesh, newNormal1, new Vector3[] { _leftPoint1, newVertex1, newVertex2 },
        new Vector2[] { _leftUV1, newUv1, newUv2 },
        new Vector3[] { _leftNormal1, newNormal1, newNormal2 });

        // second triangle
        Add_Left_triangle(submesh, newNormal2, new Vector3[] { _leftPoint1, _leftPoint2, newVertex2 },
        new Vector2[] { _leftUV1, _leftUV2, newUv2 },
        new Vector3[] { _leftNormal1, _leftNormal2, newNormal2 });

        // first triangle
        Add_Right_triangle(submesh, newNormal1, new Vector3[] { _rightPoint1, newVertex1, newVertex2 },
        new Vector2[] { _rightUV1, newUv1, newUv2 },
        new Vector3[] { _rightNormal1, newNormal1, newNormal2 });

        // second triangle
        Add_Right_triangle(submesh, newNormal2, new Vector3[] { _rightPoint1, _rightPoint2, newVertex2 },
        new Vector2[] { _rightUV1, _rightUV2, newUv2 },
        new Vector3[] { _rightNormal1, _rightNormal2, newNormal2 });

    }

    private static void Add_Left_triangle(int subMesh, Vector3 faceNormal, Vector3[] points, Vector2[] uvs, Vector3[] normals)
    {

        int p1 = 0;
        int p2 = 1;
        int p3 = 2;

        Vector3 calculatedNormal = Vector3.Cross((points[1] - points[0]).normalized, (points[2] - points[0]).normalized);

        if (Vector3.Dot(calculatedNormal, faceNormal) < 0)
        {

            p1 = 2;
            p2 = 1;
            p3 = 0;
        }

        LEFT_GATHER_ADDED_POINTS[subMesh].Add(points[p1]);
        LEFT_GATHER_ADDED_POINTS[subMesh].Add(points[p2]);
        LEFT_GATHER_ADDED_POINTS[subMesh].Add(points[p3]);

        LEFT_GATHER_ADDED_UVS[subMesh].Add(uvs[p1]);
        LEFT_GATHER_ADDED_UVS[subMesh].Add(uvs[p2]);
        LEFT_GATHER_ADDED_UVS[subMesh].Add(uvs[p3]);

        LEFT_GATHER_ADDED_NORMALS[subMesh].Add(normals[p1]);
        LEFT_GATHER_ADDED_NORMALS[subMesh].Add(normals[p2]);
        LEFT_GATHER_ADDED_NORMALS[subMesh].Add(normals[p3]);

    }

    static void Add_Right_triangle(int submesh, Vector3 faceNormal, Vector3[] points, Vector2[] uvs, Vector3[] normals)
    {


        int p1 = 0;
        int p2 = 1;
        int p3 = 2;

        Vector3 calculatedNormal = Vector3.Cross((points[1] - points[0]).normalized, (points[2] - points[0]).normalized);

        if (Vector3.Dot(calculatedNormal, faceNormal) < 0)
        {

            p1 = 2;
            p2 = 1;
            p3 = 0;
        }


        RIGHT_GATHER_ADDED_POINTS[submesh].Add(points[p1]);
        RIGHT_GATHER_ADDED_POINTS[submesh].Add(points[p2]);
        RIGHT_GATHER_ADDED_POINTS[submesh].Add(points[p3]);

        RIGHT_GATHER_ADDED_UVS[submesh].Add(uvs[p1]);
        RIGHT_GATHER_ADDED_UVS[submesh].Add(uvs[p2]);
        RIGHT_GATHER_ADDED_UVS[submesh].Add(uvs[p3]);

        RIGHT_GATHER_ADDED_NORMALS[submesh].Add(normals[p1]);
        RIGHT_GATHER_ADDED_NORMALS[submesh].Add(normals[p2]);
        RIGHT_GATHER_ADDED_NORMALS[submesh].Add(normals[p3]);

    }


    static void ResetFinalArrays()
    {

        LEFT_FINAL_SUB_INDICES[0].Clear();
        LEFT_FINAL_SUB_INDICES[1].Clear();
        LEFT_FINAL_VERTICES.Clear();
        LEFT_FINAL_NORMALS.Clear();
        LEFT_FINAL_UVS.Clear();

        RIGHT_FINAL_SUB_INDICES[0].Clear();
        RIGHT_FINAL_SUB_INDICES[1].Clear();
        RIGHT_FINAL_VERTICES.Clear();
        RIGHT_FINAL_NORMALS.Clear();
        RIGHT_FINAL_UVS.Clear();

    }

    static void SetFinalArrays_withOriginals()
    {

        int p = 0;

        for (int submesh = 0; submesh < 2; submesh++)
        {

            for (int i = 0; i < LEFT_GATHER_SUB_INDICES[submesh].Count; i++)
            {

                p = LEFT_GATHER_SUB_INDICES[submesh][i];

                LEFT_FINAL_VERTICES.Add(_victimMesh.vertices[p]);
                LEFT_FINAL_SUB_INDICES[submesh].Add(LEFT_FINAL_VERTICES.Count - 1);
                LEFT_FINAL_NORMALS.Add(_victimMesh.normals[p]);
                LEFT_FINAL_UVS.Add(_victimMesh.uv[p]);

            }

            for (int i = 0; i < RIGHT_GATHER_SUB_INDICES[submesh].Count; i++)
            {

                p = RIGHT_GATHER_SUB_INDICES[submesh][i];

                RIGHT_FINAL_VERTICES.Add(_victimMesh.vertices[p]);
                RIGHT_FINAL_SUB_INDICES[submesh].Add(RIGHT_FINAL_VERTICES.Count - 1);
                RIGHT_FINAL_NORMALS.Add(_victimMesh.normals[p]);
                RIGHT_FINAL_UVS.Add(_victimMesh.uv[p]);

            }

        }

    }

    static void AddNewTriangles_toFinalArrays()
    {

        for (int submesh = 0; submesh < 2; submesh++)
        {

            int count = LEFT_FINAL_VERTICES.Count;
            // add the new ones
            for (int i = 0; i < LEFT_GATHER_ADDED_POINTS[submesh].Count; i++)
            {

                LEFT_FINAL_VERTICES.Add(LEFT_GATHER_ADDED_POINTS[submesh][i]);
                LEFT_FINAL_SUB_INDICES[submesh].Add(i + count);
                LEFT_FINAL_UVS.Add(LEFT_GATHER_ADDED_UVS[submesh][i]);
                LEFT_FINAL_NORMALS.Add(LEFT_GATHER_ADDED_NORMALS[submesh][i]);

            }

            count = RIGHT_FINAL_VERTICES.Count;

            for (int i = 0; i < RIGHT_GATHER_ADDED_POINTS[submesh].Count; i++)
            {

                RIGHT_FINAL_VERTICES.Add(RIGHT_GATHER_ADDED_POINTS[submesh][i]);
                RIGHT_FINAL_SUB_INDICES[submesh].Add(i + count);
                RIGHT_FINAL_UVS.Add(RIGHT_GATHER_ADDED_UVS[submesh][i]);
                RIGHT_FINAL_NORMALS.Add(RIGHT_GATHER_ADDED_NORMALS[submesh][i]);

            }
        }

    }

    private static readonly List<Vector3> CAP_VERT_TRACKER = new List<Vector3>();
    private static readonly List<Vector3> CAP_VERTPOLYGON = new List<Vector3>();

    static void MakeCaps()
    {

        CAP_VERT_TRACKER.Clear();

        for (int i = 0; i < CREATED_VERTEX_POINTS.Count; i++)
            if (!CAP_VERT_TRACKER.Contains(CREATED_VERTEX_POINTS[i]))
            {
                CAP_VERTPOLYGON.Clear();
                CAP_VERTPOLYGON.Add(CREATED_VERTEX_POINTS[i]);
                CAP_VERTPOLYGON.Add(CREATED_VERTEX_POINTS[i + 1]);

                CAP_VERT_TRACKER.Add(CREATED_VERTEX_POINTS[i]);
                CAP_VERT_TRACKER.Add(CREATED_VERTEX_POINTS[i + 1]);


                bool isDone = false;
                while (!isDone)
                {
                    isDone = true;

                    for (int k = 0; k < CREATED_VERTEX_POINTS.Count; k += 2)
                    { // go through the pairs

                        if (CREATED_VERTEX_POINTS[k] == CAP_VERTPOLYGON[CAP_VERTPOLYGON.Count - 1] && !CAP_VERT_TRACKER.Contains(CREATED_VERTEX_POINTS[k + 1]))
                        { // if so add the other

                            isDone = false;
                            CAP_VERTPOLYGON.Add(CREATED_VERTEX_POINTS[k + 1]);
                            CAP_VERT_TRACKER.Add(CREATED_VERTEX_POINTS[k + 1]);

                        }
                        else if (CREATED_VERTEX_POINTS[k + 1] == CAP_VERTPOLYGON[CAP_VERTPOLYGON.Count - 1] && !CAP_VERT_TRACKER.Contains(CREATED_VERTEX_POINTS[k]))
                        {// if so add the other

                            isDone = false;
                            CAP_VERTPOLYGON.Add(CREATED_VERTEX_POINTS[k]);
                            CAP_VERT_TRACKER.Add(CREATED_VERTEX_POINTS[k]);
                        }
                    }
                }

                FillCap(CAP_VERTPOLYGON);

            }

    }

    static void FillCap(List<Vector3> vertices)
    {

        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();

        Vector3 center = Vector3.zero;
        foreach (Vector3 point in vertices)
            center += point;

        center = center / vertices.Count;


        Vector3 upward = Vector3.zero;
        // 90 degree turn
        upward.x = _blade.normal.y;
        upward.y = -_blade.normal.x;
        upward.z = _blade.normal.z;
        Vector3 left = Vector3.Cross(_blade.normal, upward);

        Vector3 displacement = Vector3.zero;
        Vector3 relativePosition = Vector3.zero;

        for (int i = 0; i < vertices.Count; i++)
        {

            displacement = vertices[i] - center;
            relativePosition = Vector3.zero;
            relativePosition.x = 0.5f + Vector3.Dot(displacement, left);
            relativePosition.y = 0.5f + Vector3.Dot(displacement, upward);
            relativePosition.z = 0.5f + Vector3.Dot(displacement, _blade.normal);

            uvs.Add(new Vector2(relativePosition.x, relativePosition.y));
            normals.Add(_blade.normal);
        }


        vertices.Add(center);
        normals.Add(_blade.normal);
        uvs.Add(new Vector2(0.5f, 0.5f));

        Vector3 calculatedNormal = Vector3.zero;
        int otherIndex = 0;
        for (int i = 0; i < vertices.Count; i++)
        {

            otherIndex = (i + 1) % (vertices.Count - 1);

            calculatedNormal = Vector3.Cross((vertices[otherIndex] - vertices[i]).normalized,
                                              (vertices[vertices.Count - 1] - vertices[i]).normalized);

            if (Vector3.Dot(calculatedNormal, _blade.normal) < 0)
            {

                triangles.Add(vertices.Count - 1);
                triangles.Add(otherIndex);
                triangles.Add(i);
            }
            else
            {

                triangles.Add(i);
                triangles.Add(otherIndex);
                triangles.Add(vertices.Count - 1);
            }

        }

        int index = 0;
        for (int i = 0; i < triangles.Count; i++)
        {

            index = triangles[i];
            RIGHT_FINAL_VERTICES.Add(vertices[index]);
            RIGHT_FINAL_SUB_INDICES[1].Add(RIGHT_FINAL_VERTICES.Count - 1);
            RIGHT_FINAL_NORMALS.Add(normals[index]);
            RIGHT_FINAL_UVS.Add(uvs[index]);

        }

        for (int i = 0; i < normals.Count; i++)
        {
            normals[i] = -normals[i];
        }

        int temp1, temp2;
        for (int i = 0; i < triangles.Count; i += 3)
        {

            temp1 = triangles[i + 2];
            temp2 = triangles[i];

            triangles[i] = temp1;
            triangles[i + 2] = temp2;
        }

        for (int i = 0; i < triangles.Count; i++)
        {

            index = triangles[i];
            LEFT_FINAL_VERTICES.Add(vertices[index]);
            LEFT_FINAL_SUB_INDICES[1].Add(LEFT_FINAL_VERTICES.Count - 1);
            LEFT_FINAL_NORMALS.Add(normals[index]);
            LEFT_FINAL_UVS.Add(uvs[index]);

        }

    }

}