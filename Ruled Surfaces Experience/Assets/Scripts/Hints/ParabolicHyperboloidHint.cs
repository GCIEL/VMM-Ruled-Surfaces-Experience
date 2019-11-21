using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParabolicHyperboloidHint : RuledSurfaceHint
{
    protected override Vector3 EvaluateLeftPoint(float t)
    {
        return new Vector3(-1 + t, -1 + t, 1);
    }

    protected override Vector3 EvaluateRightPoint(float t)
    {
        return new Vector3(-1 + t, 1 - t, -1);
    }
}
