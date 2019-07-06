using System;
using UnityEngine;
using System.Collections;

public class ThrowSimulation : MonoBehaviour
{
    public Transform Target;
    public float firingAngle = 45.0f;
    public float gravity = 9.8f;

    public Transform Projectile;
    private Transform myTransform;


    void Awake()
    {
        myTransform = transform;
    }


    void Start()
    {
        StartCoroutine(nameof(SimulateProjectile));
    }


    IEnumerator SimulateProjectile()
    {
        // Short delay added before Projectile is thrown
        yield return new WaitForSeconds(1.5f);

        // Move projectile to the position of throwing object + add some offset if needed.
        var projectilePosition = Projectile.position = myTransform.position + new Vector3(0, 0.0f, 0);

        // Calculate distance to target
        var position = Target.position;
        float targetDistance = Vector3.Distance(Projectile.position, position);

        // Calculate the velocity needed to throw the object to the target at specified angle.
        float projectileVelocity = targetDistance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);

        // Extract the X Y component of the velocity
        float vx = Mathf.Sqrt(projectileVelocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        float vy = Mathf.Sqrt(projectileVelocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

        // Calculate flight time.
        float flightDuration = targetDistance / vx;

        // Rotate projectile to face the target.
        Projectile.rotation = Quaternion.LookRotation(position - projectilePosition);

        float elapseTime = 0;

        while (elapseTime < flightDuration)
        {
            Projectile.Translate(0, (vy - (gravity * elapseTime)) * Time.deltaTime, vx * Time.deltaTime);

            elapseTime += Time.deltaTime;

            yield return null;
        }
    }
}