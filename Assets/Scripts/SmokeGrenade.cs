using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeGrenade : MonoBehaviour
{
    public GameObject smokeObject;
    public float fuseTime = 5f;
    public float smokeDuration = 15f;

    private float timer;
    private Vector3 m_ThrowDirection;
    private ParticleSystem m_ParticleSystem;
    private Rigidbody m_Rigidbody;
    private Transform projectile;
    private Vector3 target;
    private float throwingAngle;
    private float gravity;
    private bool thrown = false;
    private bool stopping = false;


    void Start()
    {
        m_ParticleSystem = smokeObject.GetComponent<ParticleSystem>();
        var m = m_ParticleSystem.main;
        m.startDelay = fuseTime;
        m_Rigidbody = GetComponent<Rigidbody>();

        projectile = gameObject.transform;
    }


    void Update()
    {
        timer += 1 * Time.deltaTime;

        if (timer >= smokeDuration + fuseTime && !stopping)
        {
            stopping = true;
            StartCoroutine(nameof(RemoveGrenade));
        }
    }


    void Throw(ThrowParameters tParams)
    {
        target = tParams.target;
        throwingAngle = tParams.throwAngle;
        gravity = tParams.gravity;

        StartCoroutine(nameof(SimulateProjectile));
    }


    void OnDrawGizmos()
    {
        var restoreColor = GUI.color;
        GUI.color = Color.green;
        UnityEditor.Handles.Label(gameObject.transform.position, "IsPlaying = " + m_ParticleSystem.isPlaying);
        if (!thrown)
        {
            Gizmos.DrawSphere(target, 0.1f); //TODO: Make the target sphere stop rendering when the throw is complete
        }

        GUI.color = restoreColor;
    }


    IEnumerator RemoveGrenade()
    {
        m_ParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        Debug.Log("Stopping grenade particle effect...");
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
        Debug.Log("Destroyed grenade object.");
    }

    IEnumerator SimulateProjectile()
    {
        Debug.Log("Beginning projectile throw simulation.");

        // Short delay added before Projectile is thrown
        //yield return new WaitForSeconds(1.5f);

        // Move projectile to the position of throwing object + add some offset if needed.
        var position = transform.position;
        var projectilePosition = position + new Vector3(0, 0.0f, 0);
        Debug.Log(projectilePosition);

        // Calculate distance to target
        float targetDistance = Vector3.Distance(position, target);
        Debug.Log(targetDistance);

        // Calculate the velocity needed to throw the object to the target at specified angle.
        float projectileVelocity = targetDistance / (Mathf.Sin(2 * throwingAngle * Mathf.Deg2Rad) / gravity);

        // Extract the X Y component of the velocity
        float vx = Mathf.Sqrt(projectileVelocity) * Mathf.Cos(throwingAngle * Mathf.Deg2Rad);
        float vy = Mathf.Sqrt(projectileVelocity) * Mathf.Sin(throwingAngle * Mathf.Deg2Rad);

        // Calculate flight time.
        float flightDuration = targetDistance / vx;

        // Rotate projectile to face the target.
        transform.rotation = Quaternion.LookRotation(target - projectilePosition);

        float elapseTime = 0;

        while (elapseTime < flightDuration)
        {
            transform.Translate(0, (vy - (gravity * elapseTime)) * Time.deltaTime, vx * Time.deltaTime);
            elapseTime += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Throw simulation complete.");
        thrown = true;
        m_Rigidbody.useGravity = true;
    }
}