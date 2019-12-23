using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolyPolySat : MonoBehaviour
{
    public Transform PolyA;
    public Transform PolyB;
    public Transform Axis;
    public List<Vector2> PolyAVertices;
    public List<Vector2> PolyBVertices;

    public List<float> ProjectPointsOntoAxis(List<Vector2> points)
    {
        Vector2 axis = Axis.localRotation * Vector3.right;
        Vector2 norm = axis.normalized;
        List<float> toRet = new List<float>();
        foreach (var point in points)
        {
            toRet.Add(Vector2.Dot(norm, point));
        }
        return toRet;
    }

    private void DrawProjectedPoints(List<float> points, Color axisCol, Color pointCol)
    {
        Vector2 axis = Axis.localRotation * Vector3.right;
        if (points.Count == 0) return;
        Vector2 norm = axis.normalized;
        Vector2 perp = new Vector2(-norm.y, norm.x);
        float distance = Vector2.Dot(perp, Axis.position);
        Vector2 drawOrigin = distance * perp;
        float minPoint = points[0];
        float maxPoint = points[0];
        Gizmos.color = pointCol;
        foreach (var point in points)
        {
            if (point < minPoint) minPoint = point;
            if (point > maxPoint) maxPoint = point;
            Gizmos.DrawWireSphere(drawOrigin + (norm * point), 0.05f);
        }
        Gizmos.color = axisCol;
        Gizmos.DrawLine(drawOrigin + (norm * (minPoint - 0.25f)), drawOrigin + (norm * (maxPoint + 0.25f)));
    }

    private void DrawPolygon(Vector2 position, List<Vector2> vertices)
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            int j = i - 1;
            if (j < 0) j = vertices.Count - 1;
            Gizmos.DrawLine(position + vertices[j], position + vertices[i]);
        }
    }

    private List<Vector2> ConvertToWorld(Vector2 origin, List<Vector2> vertices)
    {
        List<Vector2> toRet = new List<Vector2>();
        foreach (var vert in vertices)
        {
            toRet.Add(vert + origin);
        }
        return toRet;
    }

    private void DrawPolyProjections()
    {
        for (int i = 0; i < PolyAVertices.Count; i++)
        {
            int j = i - 1;
            if (j < 0) j = PolyAVertices.Count - 1;
            Vector2 axis = PolyAVertices[i] - PolyAVertices[j];
            DrawProjectedPoints(ProjectPointsOntoAxis(ConvertToWorld(PolyA.position, PolyAVertices)), Color.blue, Color.blue);
            DrawProjectedPoints(ProjectPointsOntoAxis(ConvertToWorld(PolyB.position, PolyBVertices)), Color.blue, Color.green);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        DrawPolygon(PolyA.position, PolyAVertices);
        Gizmos.color = Color.green;
        DrawPolygon(PolyB.position, PolyBVertices);

        DrawPolyProjections();
    }
}
