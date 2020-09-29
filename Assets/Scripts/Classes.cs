using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowParameters
{
    public Vector3 target;
    public float throwAngle;
    public float gravity;


    public ThrowParameters(Vector3 t, float a)
    {
        target = t;
        throwAngle = a;
    }
}

public class PlayerData
{
    public float health;


    // TODO: Find out if there's a way to have the constructor parameters be the same as the properties they're setting so that it's clear what each property is when calling the constructor
    public PlayerData(float h) { health = h; }
}