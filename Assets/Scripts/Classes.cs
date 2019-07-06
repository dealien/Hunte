using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowParameters
{
    public Vector3 target;
    public float throwAngle;
    public float gravity;

    public ThrowParameters(Vector3 t, float a, float g)
    {
        target = t;
        throwAngle = a;
        gravity = g;
    }
}