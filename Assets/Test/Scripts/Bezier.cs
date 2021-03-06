using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ベジェ曲線の一区間
public class Segment
{
    public Vector3 start, control1, control2, end;

    private int DIVISION = 10;

    public Segment(Vector3 start, Vector3 control1, Vector3 control2, Vector3 end)
    {
        this.start = start;
        this.control1 = control1;
        this.control2 = control2;
        this.end = end;
    }

    public Vector3 GetPoint(float t)
    {
        t = Mathf.Clamp01(t);
        float tx = 1.0f - t;
        return (
            start * Mathf.Pow(tx, 3) +
            control1 * 3 * Mathf.Pow(tx, 2) * t +
            control2 * 3 * Mathf.Pow(t, 2) * tx +
            end * Mathf.Pow(t, 3)
        );
    }

    public Vector3 GetPrime(float t)
    {
        t = Mathf.Clamp01(t);
        float tx = 1.0f - t;
        return (
            (control1 - start) * 3 * Mathf.Pow(tx, 2) +
            (control2 - control1) * 6 * tx * t +
            (end - control2) * 3 * Mathf.Pow(t, 2)
        );
    }

    public Vector3 GetPrimePrime(float t)
    {
        return (
            ((control2 - (control1 * 2)) + start) * 6 * (1.0f - t) +
            ((end - (control2 * 2)) + control1) * 6 * t
        );
    }

    public List<float> MapTtoRelativeDistances(int S_parts)
    {
        List<float> S_t_dist = new List<float> { 0 };
        Vector3 S_t_prev = start;
        float sumLen = 0;

        for (int i = 1; i <= S_parts; i++)
        {
            Vector3 S_t_curr = GetPoint(i / S_parts);
            sumLen += (S_t_curr - S_t_prev).magnitude;
            S_t_dist.Add(sumLen);
            S_t_prev = S_t_curr;
        }

        return S_t_dist;
    }

    public float LengthToT(float length)
    {
        if (length < 0)
        {
            return 0;
        }

        List<float> map = MapTtoRelativeDistances(DIVISION);

        if (length > map[map.Count - 1])
        {
            return 1;
        }


        for (int i = 1; i <= DIVISION; i++)
        {
            if (length <= map[i])
            {
                float tMin = (i - 1) / DIVISION;
                float tMax = i / DIVISION;
                float lenMin = map[i - 1];
                float lenMax = map[i];

                return ((length - lenMin) / (lenMax - lenMin)) * (tMax - tMin) + tMin;
            }
        }

        return float.NaN;
    }

    public float Length
    {
        get
        {
            List<float> map = MapTtoRelativeDistances(DIVISION);
            return map[map.Count - 1];
        }
    }


}

// ベジェ曲線全体(連続したsegmentの集合)
public class Bezier
{
    private List<Segment> segments = new List<Segment>();
    private int DIVISION = 10;

    public int NumSegments
    {
        get
        {
            return segments.Count;
        }
    }

    public int NumPoints
    {
        get
        {
            return 4 + (segments.Count - 1) * 3;
        }
    }

    public List<Segment> Segments
    {
        get
        {
            return segments;
        }
        set
        {
            segments = value;
        }
    }

    public List<Vector3> ControlPoints
    {
        get
        {
            var points = new List<Vector3>();
            for (int i = 0; i < segments.Count; i++)
            {
                points.Add(segments[i].start);
                points.Add(segments[i].control1);
                points.Add(segments[i].control2);
            }
            if (segments.Count > 0) points.Add(segments[segments.Count - 1].end);

            return points;
        }

        set
        {
            if (value.Count < 4 || (value.Count - 1) % 3 != 0)
            {
                throw new System.ArgumentException("The number of control points must be 3N+1.");
            }

            segments = new List<Segment>();
            for (int i = 0; i < (value.Count - 1) / 3; i++)
            {
                AddSegment(new Segment(
                    value[i * 3],
                    value[i * 3 + 1],
                    value[i * 3 + 2],
                    value[i * 3 + 3]));
            }
        }
    }

    public bool AddSegment(Segment segment)
    {
        var last = segments.Count - 1 > 0 ? segments[segments.Count - 1] : null;
        if (last != null && last.end != segment.start)
        {
            return false;
        }
        else
        {
            segments.Add(segment);
            return true;
        }
    }

    public void AddSegments(List<Segment> segments)
    {
        for (int i = 0; i < segments.Count; i++)
        {
            AddSegment(segments[i]);
        }
    }

    public Vector3 GetPoint(float t)
    {
        if (segments.Count < 1)
        {
            // nullを返したいけどVector3はnull許容じゃないしどうすればいいんですか(質問)
            return Vector3.zero;
        }
        if (t < 0)
        {
            return segments[0].start;
        }
        var index = Mathf.FloorToInt(t);
        if (index > segments.Count)
        {
            return segments[segments.Count - 1].end;
        }
        return segments[index].GetPoint(t - index);
    }

    public List<Vector3> GetAllPoints(int divisions)
    {
        divisions = Mathf.Max(1, divisions);
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < segments.Count; i++)
        {
            for (int j = 0; j < divisions; j++)
            {
                float t = j / (float)divisions;
                points.Add(segments[i].GetPoint(t));
            }
        }
        points.Add(segments[segments.Count - 1].end);
        return points;
    }

    public List<float> MapTtoRelativeDistances(int S_parts)
    {
        List<float> map = new List<float>();
        float currLength = 0f;
        for (int i = 0; i < segments.Count; i++)
        {
            List<float> segMap = segments[i].MapTtoRelativeDistances(S_parts);
            for (int j = 0; j < segMap.Count - 1; j++)
            {
                map.Add(segMap[j] + currLength);
            }
            currLength = segMap[segMap.Count - 1];
        }
        return map;
    }

    public float LengthToT(float length)
    {
        if (length < 0)
        {
            return 0;
        }

        List<float> map = MapTtoRelativeDistances(DIVISION);

        if (length > map[map.Count - 1])
        {
            return segments.Count;
        }

        for (int i = 1; i <= DIVISION; i++)
        {
            if (length <= map[i])
            {
                float tMin = (i - 1) / DIVISION;
                float tMax = i / DIVISION;
                float lenMin = map[i - 1];
                float lenMax = map[i];

                return (((length - lenMin) / (lenMax - lenMin)) * (tMax - tMin) + tMin) * segments.Count;
            }
        }

        return float.NaN;
    }

    public float Length
    {
        get
        {
            List<float> map = MapTtoRelativeDistances(DIVISION);
            return map[map.Count - 1];
        }
    }

}
