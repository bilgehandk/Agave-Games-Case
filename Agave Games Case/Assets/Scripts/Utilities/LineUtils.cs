using UnityEngine;
using System.Collections.Generic;
public static class LineUtils
{
    private const int DefaultSegmentsPerCurve = 10;
    private const float ControlPointRatio = 0.3f;
    public static List<Vector3> GenerateSmoothCurve(List<Vector3> points, int segmentsPerCurve = DefaultSegmentsPerCurve)
    {
        if (points == null || points.Count < 2)
            return points ?? new List<Vector3>();
        List<Vector3> smoothPoints = new List<Vector3>();
        smoothPoints.Add(points[0]);
        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector3 p0 = points[i];
            Vector3 p1 = points[i + 1];
            Vector3 control0, control1;
            if (i > 0 && i < points.Count - 2)
            {
                Vector3 prev = points[i - 1];
                Vector3 next = points[i + 2];
                Vector3 tangent0 = (p1 - prev).normalized;
                Vector3 tangent1 = (next - p0).normalized;
                float distance = Vector3.Distance(p0, p1);
                control0 = p0 + tangent0 * (distance * ControlPointRatio);
                control1 = p1 - tangent1 * (distance * ControlPointRatio);
            }
            else
            {
                control0 = p0 + (p1 - p0) * ControlPointRatio;
                control1 = p1 - (p1 - p0) * ControlPointRatio;
            }
            for (int j = 1; j <= segmentsPerCurve; j++)
            {
                float t = j / (float)segmentsPerCurve;
                smoothPoints.Add(CalculateBezierPoint(p0, control0, control1, p1, t));
            }
        }
        return smoothPoints;
    }
    private static Vector3 CalculateBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;
        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;
        return p;
    }
    public static void ApplySmoothCurve(LineRenderer lineRenderer, List<Vector3> points, int segmentsPerCurve = DefaultSegmentsPerCurve)
    {
        if (lineRenderer == null || points == null || points.Count < 2)
            return;
        List<Vector3> smoothPoints = GenerateSmoothCurve(points, segmentsPerCurve);
        lineRenderer.positionCount = smoothPoints.Count;
        for (int i = 0; i < smoothPoints.Count; i++)
        {
            lineRenderer.SetPosition(i, smoothPoints[i]);
        }
    }
}
