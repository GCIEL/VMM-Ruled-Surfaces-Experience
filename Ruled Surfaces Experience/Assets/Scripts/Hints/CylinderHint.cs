using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderHint : RuledSurfaceHint
{
    protected override Vector3 EvaluateLeftPoint(float t)
    {
        return 2 * new Vector3(Mathf.Cos(t), Mathf.Sin(t), 1);
    }

    protected override Vector3 EvaluateRightPoint(float t)
    {
        return 2 * new Vector3(Mathf.Cos(t), Mathf.Sin(t), -1);
    }
}