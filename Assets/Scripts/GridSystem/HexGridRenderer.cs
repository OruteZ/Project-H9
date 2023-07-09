using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class HexGridRenderer : MonoBehaviour
{
    private Mesh _mesh;
    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;

    private List<Face> _faces;

    public Material material;

    [Header("size info")] 
    public float innerSize;
    public float outerSize;
    public float height;
    public bool isFlatTopped;
    
    
    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();

        _mesh = new Mesh
        {
            name = "Hex"
        };

        _meshFilter.mesh = _mesh;
        _meshRenderer.material = material;
    }
    
    public void SetMaterial(Material mat)
    {
        _meshRenderer.material = mat;
        material = mat;
    }

    private void OnEnable()
    {
        DrawMesh();
    }

    
    // private void OnValidate()
    // {
    //     Awake();
    //     DrawMesh();
    // }

    public void DrawMesh()
    {
        DrawFaces();
        CombineFaces();
    }

    private void DrawFaces()
    {
        _faces = new List<Face>();
        
        //Top faces
        for (int point = 0; point < 6; point++)
        {
            _faces.Add(CreateFace(innerSize, outerSize, height * 0.5f, height * 0.5f, point));
        }
        
        //Bottom Faces
        for (int point = 0; point < 6; point++)
        {
            _faces.Add(CreateFace(innerSize, outerSize, -height * 0.5f, -height * 0.5f, point, true));
        }
        
        //Outer faces
        for (int point = 0; point < 6; point++)
        {
            _faces.Add(CreateFace(outerSize, outerSize, height * 0.5f, -height * 0.5f, point, true));
        }
        
        //Inner faces
        for (int point = 0; point < 6; point++)
        {
            _faces.Add(CreateFace(innerSize, innerSize, height * 0.5f, -height * 0.5f, point));
        }
    }

    private void CombineFaces()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (int i = 0; i < _faces.Count; i++)
        {
            //vertices 추가
            vertices.AddRange(_faces[i].vertices);
            uvs.AddRange(_faces[i].uvs);
            
            //Offset the triangles
            int offset = (4 * i);
            foreach (int triangle in _faces[i].triangles)
            {
                tris.Add(triangle + offset);
            }
        }

        _mesh.vertices = vertices.ToArray();
        _mesh.triangles = tris.ToArray();
        _mesh.uv = uvs.ToArray();
        _mesh.RecalculateNormals();
    }

    private Face CreateFace(float innerRad, float outerRad, float heightA, float heightB, int point,
        bool reverse = false)
    {
        Vector3 pointA = GetPoint(innerRad, heightB, point);
        Vector3 pointB = GetPoint(innerRad, heightB, (point < 5) ? point + 1 : 0);
        Vector3 pointC = GetPoint(outerRad, heightA, (point < 5) ? point + 1 : 0);
        Vector3 pointD = GetPoint(outerRad, heightA, point);

        var vertices = new List<Vector3>() { pointA, pointB, pointC, pointD };
        var triangles = new List<int>() { 0, 1, 2, 2, 3, 0 };
        var uvs = new List<Vector2>() { Vector2.zero, Vector2.right, Vector2.one, Vector2.up };

        if (reverse)
        {
            vertices.Reverse();
        }

        return new Face(vertices, triangles, uvs);
    }

    protected Vector3 GetPoint(float size, float height, int index)
    {
        float angleDeg = isFlatTopped ? 60 * index : 60 * index - 30;
        float angleRad = Mathf.Deg2Rad * angleDeg;

        return new Vector3(size * Mathf.Cos(angleRad), height, size * Mathf.Sin(angleRad));
    }
}

public struct Face
{
    public List<Vector3> vertices { get; private set; }
    public List<int> triangles { get; private set; }
    public List<Vector2> uvs { get; private set; }

    public Face(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
    {
        this.vertices = vertices;
        this.triangles = triangles;
        this.uvs = uvs;
    }
}
