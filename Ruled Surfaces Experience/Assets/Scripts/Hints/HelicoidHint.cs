using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicoidHint : RuledSurfaceHint
{
    private float offset = 1.16f;

    protected override Vector3 EvaluateLeftPoint(float t)
    {
        return new Vector3(2 * Mathf.Cos(t + offset), t-2, 2 * Mathf.Sin(t + offset));
    }

    protected override Vector3 EvaluateRightPoint(float t)
    {
        return new Vector3(2 * -Mathf.Cos(t + offset), t-2, 2 * -Mathf.Sin(t + offset));
    }
}
