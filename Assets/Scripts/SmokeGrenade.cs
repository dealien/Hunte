using System.Collections;
using UnityEditor;
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
    private Vector3 target;
    private float throwingAngle;
    private float gravity;
    private bool stopping;


    void Start()
    {
        m_ParticleSystem = smokeObject.GetComponent<ParticleSystem>();
        var m = m_ParticleSystem.main;
        m.startDelay = fuseTime;
        m.duration = smokeDuration;
        m_Rigidbody = GetComponent<Rigidbody>();
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


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
        }


        void Throw(ThrowParameters tParams)
        {
            target = tParams.target;
            throwingAngle = tParams.throwAngle;
            m_Rigidbody = GetComponent<Rigidbody>();

            gravity = Mathf.Abs(Physics.gravity.y);
            StartCoroutine(nameof(ThrowGrenade));
        }


        void OnDrawGizmos()
        {
            var restoreColor = GUI.color;
            GUI.color = Color.green;
            Handles.Label(gameObject.transform.position,
                $"IsPlaying = {m_ParticleSystem.isPlaying}\nTimer = {timer}\nEmitting = {!stopping}");
            GUI.color = restoreColor;
        }
    }


    private IEnumerator RemoveGrenade()
    {
        m_ParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        Debug.Log("Stopping grenade particle effect...");
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
        Debug.Log("Destroyed grenade object.");
    }


    private IEnumerator ThrowGrenade()
    {
        Debug.Log("Throwing projectile...");
        Debug.Log($"Gravity: {Physics.gravity}({gravity})");

        // Short delay added before Projectile is thrown
        //yield return new WaitForSeconds(1.5f);

        var projectilePosition = transform.position;
        Debug.Log($"Current position: {projectilePosition}");

        // Calculate distance to target
        float targetDistance = Vector3.Distance(projectilePosition, target);
        Debug.Log($"Distance to target: {targetDistance}");

        // Calculate the velocity needed to throw the object to the target at specified angle.
        float projectileVelocity = targetDistance / (Mathf.Sin(2 * throwingAngle * Mathf.Deg2Rad) / gravity);

        // Extract the X Y component of the velocity
        float vx = Mathf.Sqrt(projectileVelocity) * Mathf.Cos(throwingAngle * Mathf.Deg2Rad);
        float vy = Mathf.Sqrt(projectileVelocity) * Mathf.Sin(throwingAngle * Mathf.Deg2Rad);

        // Rotate projectile to face the target
        transform.rotation = Quaternion.LookRotation(target - projectilePosition);
        var rotationToTarget = Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up) * Vector3.forward;

        m_Rigidbody.velocity = (vx * rotationToTarget) + (vy * Vector3.up);
        yield return null;
    }
}