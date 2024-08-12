using UnityEngine;

public static class H9Math
{
    /// <summary>
    /// Ray와 y 평면의 교점을 계산합니다.
    /// </summary>
    /// <param name="ray"></param>
    /// <param name="t"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool IsRayIntersectingPlane(Ray ray, out float t, float y = 0)
    {
        t = 0f;

        // Check if the ray direction is parallel to the plane (y-plane)
        if (Mathf.Approximately(ray.direction.y, 0f))
        {
            // Ray is parallel to the y-plane, no intersection
            return false;
        }

        // Calculate intersection distance along the ray
        t = (y - ray.origin.y) / ray.direction.y;

        // Check if the intersection point is in front of the ray's origin
        if (t < 0)
        {
            // Intersection point is behind the ray's origin
            return false;
        }

        // Intersection point found
        return true;
    }
}