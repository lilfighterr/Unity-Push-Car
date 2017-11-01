using UnityEngine;
using System;

public class BezierSpline : MonoBehaviour {

    [SerializeField]
    private Vector3[] points;

    [SerializeField]
    private BezierControlPointMode[] modes;

    [SerializeField]
    private bool loop;

    public bool Loop {
        get {
            return loop;
        }
        set {
            loop = value;
            if (value == true) {
                modes[modes.Length - 1] = modes[0];
                SetControlPoint(0, points[0]);
            }
        }
    }

    public int ControlPointCount {
        get {
            return points.Length;
        }
    }

    public Vector3 GetControlPoint(int index) {
        return points[index];
    }

    public void SetControlPoint(int index, Vector3 point) {
        if (index % 3 == 0) {
            Vector3 delta = point - points[index];
            if (loop) {
                if (index == 0) {
                    points[1] += delta;
                    points[points.Length - 2] += delta;
                    points[points.Length - 1] = point;
                }
                else if (index == points.Length - 1) {
                    points[0] = point;
                    points[1] += delta;
                    points[index - 1] += delta;
                }
                else {
                    points[index - 1] += delta;
                    points[index + 1] += delta;
                }
            }
            else {
                if (index > 0) {
                    points[index - 1] += delta;
                }
                if (index + 1 < points.Length) {
                    points[index + 1] += delta;
                }
            }
        }
        points[index] = point;
        EnforceMode(index);
    }

    public BezierControlPointMode GetControlPointMode(int index) {
        return modes[(index + 1) / 3];
    }

    public void SetControlPointMode(int index, BezierControlPointMode mode) {
        int modeIndex = (index + 1) / 3;
        modes[modeIndex] = mode;
        if (loop) {
            if (modeIndex == 0) {
                modes[modes.Length - 1] = mode;
            }
            else if (modeIndex == modes.Length - 1) {
                modes[0] = mode;
            }
        }
        EnforceMode(index);
    }

    private void EnforceMode(int index) {
        int modeIndex = (index + 1) / 3;
        BezierControlPointMode mode = modes[modeIndex];
        if (mode == BezierControlPointMode.Free || !loop && (modeIndex == 0 || modeIndex == modes.Length - 1)) {
            return;
        }

        int middleIndex = modeIndex * 3;
        int fixedIndex, enforcedIndex;
        if (index <= middleIndex) {
            fixedIndex = middleIndex - 1;
            if (fixedIndex < 0) {
                fixedIndex = points.Length - 2;
            }
            enforcedIndex = middleIndex + 1;
            if (enforcedIndex >= points.Length) {
                enforcedIndex = 1;
            }
        }
        else {
            fixedIndex = middleIndex + 1;
            if (fixedIndex >= points.Length) {
                fixedIndex = 1;
            }
            enforcedIndex = middleIndex - 1;
            if (enforcedIndex < 0) {
                enforcedIndex = points.Length - 2;
            }
        }

        Vector3 middle = points[middleIndex];
        Vector3 enforcedTangent = middle - points[fixedIndex];
        if (mode == BezierControlPointMode.Aligned) {
            enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);
        }
        points[enforcedIndex] = middle + enforcedTangent;
    }

    public int CurveCount {
        get {
            return (points.Length - 1) / 3;
        }
    }

    public Vector3 GetPoint(float t) { //Gets point in total spline corresponding to [0,1]
        int i;
        if (t >= 1f) {
            t = 1f;
            i = points.Length - 4;
        }
        else {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }
        return transform.TransformPoint(Bezier.GetPoint(points[i], points[i + 1], points[i + 2], points[i + 3], t));
    }

    public Vector3 GetVelocity(float t) {
        int i;
        if (t >= 1f) {
            t = 1f;
            i = points.Length - 4;
        }
        else {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }
        return transform.TransformPoint(Bezier.GetFirstDerivative(points[i], points[i + 1], points[i + 2], points[i + 3], t)) - transform.position;
    }

    public Vector3 GetDirection(float t) {
        return GetVelocity(t).normalized;
    }

    public void AddCurve() {
        Vector3 point = points[points.Length - 1];
        Array.Resize(ref points, points.Length + 3);
        point.x += 1f;
        points[points.Length - 3] = point;
        point.x += 1f;
        points[points.Length - 2] = point;
        point.x += 1f;
        points[points.Length - 1] = point;

        Array.Resize(ref modes, modes.Length + 1);
        modes[modes.Length - 1] = modes[modes.Length - 2];
        EnforceMode(points.Length - 4);

        if (loop) {
            points[points.Length - 1] = points[0];
            modes[modes.Length - 1] = modes[0];
            EnforceMode(0);
        }
    }

    public void Reset() {
        points = new Vector3[] {
            new Vector3(1f, 0f, 0f),
            new Vector3(2f, 0f, 0f),
            new Vector3(3f, 0f, 0f),
            new Vector3(4f, 0f, 0f)
        };
        modes = new BezierControlPointMode[] {
            BezierControlPointMode.Free,
            BezierControlPointMode.Free
        };
    }

    public float ClosestTimeOnBezier(Vector3 point) //Returns t of bezier where point in worldspace is closest to (multiple iterations)
    {
        float t = BestFitTime(point, 0, 1, 100);
        float delta = 1.0f / 10.0f;
        for (int i = 0; i < 4; i++)
        {
            t = BestFitTime(point, t - delta, t + delta, 100);
            delta /= 10;
        }
        return t;
    }

    public float BestFitTime(Vector3 point, float start, float end, int steps) //Calculate best fit time in given interval
    {

        start = Mathf.Clamp01(start);
        end = Mathf.Clamp01(end);
        float step = (end - start) / (float)steps;
        float Res = 0;
        float Ref = float.MaxValue; //will be replaced by distance of bezier(t) to point
        for (int i = 0; i < steps; i++)
        {
            float t = start + step * i;
            float L = (GetPoint(t) - point).sqrMagnitude;
            if (L < Ref)
            {
                Ref = L;
                Res = t;
            }
        }
        return Res;
    }

    public Vector3 ClosestPointOnBezier(Vector3 point)
    {
        return GetPoint(ClosestTimeOnBezier(point));
    }

    public Vector3 OnLine(Vector3 point) //If on line, return 0f, else return point closest to line
    {
        Vector3 closestPoint = ClosestPointOnBezier(point);
        if (closestPoint == point)
        {
            return Vector3.zero;
        }
        return closestPoint;
    }

    public float SplineLength() //Returns spline total length
    {
        float length = 0;
        float start = 0;
        float steps = 100f; //Amount of times we'll divide the spline
        for (float i = 1f; i <= steps; i++)
        {
            length += (GetPoint(i / steps) - GetPoint(start)).magnitude;
            start = i / steps;
        }
        return length;
    }
    public float CurrentLength(float t) //Returns current length travelled through spline from start
    {
        float length = 0 , start = 0 , next = 0;
        float end = t;
        float steps = 100f; //Amount of times we'll divide the spline
        float step = end / steps;

        for (float i = 1f; i <= steps; i++)
        {
            next = step * i;
            length += (GetPoint(next) - GetPoint(start)).magnitude;
            start = next;
        }
        return length;
    }
}