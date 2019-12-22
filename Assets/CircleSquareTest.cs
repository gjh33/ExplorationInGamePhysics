using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleSquareTest : MonoBehaviour
{
    public const float EPS = 0.00001f;

    public Transform Circle;
    public Transform Square;
    public Vector2 SquareSize;
    public float CircleRadius;
    public bool resolve;

    bool colliding = false;
    Vector2 localCirclePos;
    Vector2 localProjection;
    Vector2 localDelta;
    Vector2 localMTV;
    Vector2 DeltaVector;
    Vector2 MTV;
    float distance;

    public void Intersect()
    {
        Vector2 rel = Circle.position - Square.position;
        float angle = -Square.rotation.eulerAngles.z * Mathf.Deg2Rad;
        Vector2 local = new Vector2(Mathf.Cos(angle) * rel.x - Mathf.Sin(angle) * rel.y, Mathf.Sin(angle) * rel.x + Mathf.Cos(angle) * rel.y);
        localCirclePos = local;

        Vector2 projection = local;
        bool inside = true;
        if (local.x > SquareSize.x / 2 || local.x < -SquareSize.x / 2 || local.y > SquareSize.y / 2 || local.y < -SquareSize.y / 2)
        {
            projection = new Vector2
            {
                x = Mathf.Max(-SquareSize.x / 2, Mathf.Min(local.x, SquareSize.x / 2)),
                y = Mathf.Max(-SquareSize.y / 2, Mathf.Min(local.y, SquareSize.y / 2))
            };
            inside = false;
        }
        else
        {
            float rightDist = SquareSize.x / 2 - local.x;
            float leftDist = local.x + SquareSize.x / 2;
            float upDist = SquareSize.y / 2 - local.y;
            float downDist = local.y + SquareSize.y / 2;
            float min = Mathf.Min(rightDist, leftDist, upDist, downDist);
            if (min == rightDist)
            {
                projection = new Vector2
                {
                    x = SquareSize.x / 2,
                    y = local.y
                };
            }
            else if (min == leftDist)
            {
                projection = new Vector2
                {
                    x = -SquareSize.x / 2,
                    y = local.y
                };
            }
            else if (min == upDist)
            {
                projection = new Vector2
                {
                    x = local.x,
                    y = SquareSize.y / 2
                };
            }
            else
            {
                projection = new Vector2
                {
                    x = local.x,
                    y = -SquareSize.y / 2
                };
            }
        }
        localProjection = projection;
        Vector2 delta = local - projection;
        localDelta = delta;
        localMTV = delta.normalized * (CircleRadius - delta.magnitude);
        if (inside)
        {
            localMTV = delta + delta.normalized * CircleRadius;
            localMTV *= -1;
        }

        colliding = inside || delta.sqrMagnitude < CircleRadius * CircleRadius - EPS;
        distance = inside ? -localMTV.magnitude : delta.magnitude - CircleRadius;

        MTV = new Vector2(Mathf.Cos(-angle) * localMTV.x - Mathf.Sin(-angle) * localMTV.y, Mathf.Sin(-angle) * localMTV.x + Mathf.Cos(-angle) * localMTV.y);
    }

    private void OnDrawGizmos()
    {
        Intersect();
        // Unprojected
        Matrix4x4 ogMatrix = Gizmos.matrix;
        Gizmos.color = colliding ? Color.red : Color.green;
        Gizmos.DrawWireSphere(Circle.position, CircleRadius);
        Gizmos.matrix = Matrix4x4.TRS(Square.position, Square.rotation, Square.lossyScale);
        Gizmos.DrawWireCube(Vector3.zero, SquareSize);
        // Projected
        Gizmos.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Square.lossyScale);
        Gizmos.color = Color.gray;
        Gizmos.DrawWireCube(Vector3.zero, SquareSize);
        Gizmos.matrix = ogMatrix;
        Gizmos.DrawWireSphere(localCirclePos, CircleRadius);
        Gizmos.DrawLine(localProjection, localProjection + localDelta);
        Gizmos.DrawWireSphere(localProjection, 0.1f);
        if (distance < -EPS)
        {
            Gizmos.color = new Color(0, 0, 0.5f);
            Gizmos.DrawLine(localCirclePos, localCirclePos + localMTV);
            if (resolve)
            {
                Circle.position = Circle.position + (Vector3)MTV;
            }
            else
            {
                Gizmos.DrawLine(Circle.position, Circle.position + (Vector3)MTV);
            }
        }
    }
}
