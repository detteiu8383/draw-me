using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsToBezier
{
    public Bezier FitCurve(List<Vector3> points, float maxError)
    {
        if (points.Count < 2)
        {
            return null;
        }

        int nPts = points.Count;
        Vector3 leftTangent = ComputeTangent(points[1], points[0]);
        Vector3 rightTangent = ComputeTangent(points[nPts - 2], points[nPts - 1]);

        return FitCubic(points, leftTangent, rightTangent, maxError);
    }

    private Bezier FitCubic(List<Vector3> points, Vector3 leftTangent, Vector3 rightTangent, float error)
    {
        int maxIterations = 4;

        if (points.Count == 2)
        {
            float dist = (points[0] - points[1]).magnitude / 3;
            Bezier retBezier = new Bezier
            {
                ControlPoints = new List<Vector3> {
                points[0],
                points[0] + leftTangent * dist,
                points[1] + rightTangent * dist,
                points[1]
                }
            };
            return retBezier;
        }

        List<float> u = ChordLengthParameterize(points);

        (Segment segment, float maxError, int splitPoint) = GenerateAndReport(points, u, u, leftTangent, rightTangent);

        if (maxError == 0 || maxError < error)
        {
            Bezier retBezier = new Bezier
            {
                Segments = new List<Segment> { segment }
            };
            return retBezier;
        }

        if (maxError < error * error)
        {
            List<float> uPrime = u;
            float prevErr = maxError;
            int prevSplit = splitPoint;

            for (int i = 0; i < maxIterations; i++)
            {
                uPrime = Reparameterize(segment, points, uPrime);
                (segment, maxError, splitPoint) = GenerateAndReport(points, u, uPrime, leftTangent, rightTangent);

                if (maxError < error)
                {
                    Bezier retBezier = new Bezier
                    {
                        Segments = new List<Segment> { segment }
                    };
                    return retBezier;
                }
                else if (splitPoint == prevSplit)
                {
                    float errChange = maxError / prevErr;
                    if (errChange > 0.9999f && errChange < 1.0001)
                    {
                        break;
                    }
                }

                prevErr = maxError;
                prevSplit = splitPoint;
            }
        }

        Bezier bezier = new Bezier();

        Vector3 centerVector = points[splitPoint - 1] - points[splitPoint + 1];
        if (centerVector == Vector3.zero)
        {
            centerVector = points[splitPoint - 1] - points[splitPoint];
        }
        Vector3 toCenterTangent = centerVector.normalized;
        Vector3 fromCenterTangent = toCenterTangent * -1;

        bezier.AddSegments(FitCubic(points.GetRange(0, splitPoint + 1), leftTangent, toCenterTangent, error).Segments);
        bezier.AddSegments(FitCubic(points.GetRange(splitPoint, points.Count - splitPoint), fromCenterTangent, rightTangent, error).Segments);

        return bezier;
    }

    private List<float> Reparameterize(Segment segment, List<Vector3> points, List<float> parameters)
    {
        for (int i = 0; i < parameters.Count; i++)
        {
            parameters[i] = NewtonRaphsonRootFind(segment, points[i], parameters[i]);
        }
        return parameters;
    }

    private float NewtonRaphsonRootFind(Segment segment, Vector3 point, float u)
    {
        Vector3 d = segment.GetPoint(u) - point;
        Vector3 qprime = segment.GetPrime(u);
        float numerator = Vector3.Dot(d, qprime);
        float denominator = qprime.magnitude + 2 * Vector3.Dot(d, segment.GetPrimePrime(u));

        if (denominator == 0)
        {
            return u;
        }
        else
        {
            return u - numerator / denominator;
        }
    }

    private Vector3 ComputeTangent(Vector3 pointA, Vector3 pointB)
    {
        return (pointA - pointB).normalized;
    }

    private List<float> ChordLengthParameterize(List<Vector3> points)
    {
        List<float> u = new List<float>();
        float prevU = 0;
        Vector3 prevP = Vector3.zero;

        for (int i = 0; i < points.Count; i++)
        {
            float currU = i > 0 ? prevU + (points[i] - prevP).magnitude : 0;
            u.Add(currU);
            prevU = currU;
            prevP = points[i];
        }
        for (int i = 0; i < u.Count; i++)
        {
            u[i] = u[i] / prevU;
        }

        return u;
    }

    private (Segment segment, float maxError, int splitPoint) GenerateAndReport(List<Vector3> points, List<float> paramsOrig, List<float> paramsPrime, Vector3 leftTangent, Vector3 rightTangent)
    {
        Segment segment = GenerateSegment(points, paramsPrime, leftTangent, rightTangent);

        (float maxError, int splitPoint) = ComputeMaxError(points, segment, paramsOrig);

        return (segment, maxError, splitPoint);
    }

    private Segment GenerateSegment(List<Vector3> points, List<float> parameters, Vector3 leftTangent, Vector3 rightTangent)
    {
        Vector3 firstPoint = points[0];
        Vector3 lastPoint = points[points.Count - 1];

        Segment segment = new Segment(firstPoint, Vector3.zero, Vector3.zero, lastPoint);

        List<List<Vector3>> A = new List<List<Vector3>>();
        for (int i = 0; i < parameters.Count; i++)
        {
            float u = parameters[i];
            float ux = 1 - u;
            A.Add(new List<Vector3> { leftTangent * 3 * u * (ux * ux), rightTangent * 3 * ux * (u * u) });
        }

        List<List<float>> C = new List<List<float>> { new List<float> { 0, 0 }, new List<float> { 0, 0 } };

        List<float> X = new List<float> { 0, 0 };

        for (int i = 0; i < parameters.Count; i++)
        {
            Vector3 a = A[i][0];
            Vector3 b = A[i][1];
            C[0][0] += Vector3.Dot(a, a);
            C[0][1] += Vector3.Dot(a, b);
            C[1][0] += Vector3.Dot(a, b);
            C[1][1] += Vector3.Dot(b, b);

            Vector3 tmp = points[i] - new Segment(firstPoint, firstPoint, lastPoint, lastPoint).GetPoint(parameters[i]);
            X[0] += Vector3.Dot(a, tmp);
            X[1] += Vector3.Dot(b, tmp);
        }

        float det_C0_C1 = C[0][0] * C[1][1] - C[1][0] * C[0][1];
        float det_C0_X = C[0][0] * X[1] - C[1][0] * X[0];
        float det_X_C1 = X[0] * C[1][1] - X[1] * C[0][1];

        float alpha_l = det_C0_C1 == 0 ? 0 : det_X_C1 / det_C0_C1;
        float alpha_r = det_C0_C1 == 0 ? 0 : det_C0_X / det_C0_C1;

        float segLength = (firstPoint - lastPoint).magnitude;
        float epsilon = 1.0E-6f * segLength;
        if (alpha_l < epsilon || alpha_r < epsilon)
        {
            segment.control1 = firstPoint + leftTangent * segLength / 3.0f;
            segment.control2 = lastPoint + rightTangent * segLength / 3.0f;
        }
        else
        {

            segment.control1 = firstPoint + leftTangent * alpha_l;
            segment.control2 = lastPoint + rightTangent * alpha_r;

        }

        return segment;
    }

    private (float maxError, int splitPoint) ComputeMaxError(List<Vector3> points, Segment segment, List<float> paramerters)
    {
        float maxDist = 0;
        int splitPoint = Mathf.FloorToInt(points.Count / 2);

        List<float> t_distMap = MapTtoRelativeDistances(segment, 10);

        for (int i = 1; i < points.Count - 1; i++)
        {
            Vector3 point = points[i];

            float t = Find_t(paramerters[i], t_distMap, 10);

            Vector3 v = segment.GetPoint(t) - point;
            float dist = v.magnitude;

            if (dist > maxDist)
            {
                maxDist = dist;
                splitPoint = i;
            }
        }

        return (maxDist, splitPoint);
    }

    private List<float> MapTtoRelativeDistances(Segment segment, int S_parts)
    {
        List<float> S_t_dist = new List<float> { 0 };
        Vector3 S_t_prev = segment.start;
        float sumLen = 0;

        for (int i = 1; i <= S_parts; i++)
        {
            Vector3 S_t_curr = segment.GetPoint(i / S_parts);
            sumLen += (S_t_curr - S_t_prev).magnitude;
            S_t_dist.Add(sumLen);
            S_t_prev = S_t_curr;
        }

        for (int i = 0; i < S_t_dist.Count; i++)
        {
            S_t_dist[i] = S_t_dist[i] / sumLen;
        }

        return S_t_dist;
    }
    private float Find_t(float param, List<float> t_distMap, int S_parts)
    {
        if (param < 0)
        {
            return 0;
        }
        if (param > 1)
        {
            return 1;
        }

        for (int i = 1; i <= S_parts; i++)
        {
            if (param <= t_distMap[i])
            {
                float tMin = (i - 1) / S_parts;
                float tMax = i / S_parts;
                float lenMin = t_distMap[i - 1];
                float lenMax = t_distMap[i];

                return ((param - lenMin) / (lenMax - lenMin)) * (tMax - tMin) + tMin;
            }
        }

        return float.NaN;
    }
}