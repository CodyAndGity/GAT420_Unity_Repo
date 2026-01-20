using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AutonomousAgent : AIAgent
{
    [SerializeField] Movement movement;
    [SerializeField] Perception seekPerception;
    [SerializeField] Perception fleePerception;

    //Wander parameters were generated from ChatGPT
    // --- Wander parameters (tweak in inspector) ---
    [Header("Wander")]
    [Tooltip("Radius of the wander sphere.")]
    [SerializeField] float wanderRadius = 1.2f;
    [Tooltip("Distance ahead of the agent where the wander sphere is centered.")]
    [SerializeField] float wanderDistance = 2.0f;
    [Tooltip("Max random displacement added to the wander target per second.")]
    [SerializeField] float wanderJitter = 0.8f;

    // current point on the wander sphere (local offset from sphere center)
    Vector3 wanderTarget;



    void Start()
    {
        // initialize wanderTarget to a random point on the sphere surface
        wanderTarget = Random.onUnitSphere * wanderRadius;
    }

    void Update()
    {
        if (seekPerception != null)
        {
            var gameObjects = seekPerception.GetGameObjects();
            if (gameObjects.Length > 0)
            {
                Vector3 force = Seek(gameObjects[0]);
                movement.ApplyForce(force);
            }
        }


        if (fleePerception != null)
        {
            var gameObjects = fleePerception.GetGameObjects();
            if (gameObjects.Length > 0)
            {
                Vector3 force = Flee(gameObjects[0]);
                movement.ApplyForce(force);
            }
        }

        //foreach(var go in gameObjects)
        //{
        //    Debug.DrawLine(transform.position,go.transform.position);
        //}

        //movement.ApplyForce(transform.forward);
        movement.ApplyForce(Wander());
        transform.position = Utilities.Wrap(transform.position, new Vector3(-15, -15, -15), new Vector3(15, 15, 15));
        if (movement.Velocity.sqrMagnitude > 0)
        {
            transform.rotation = Quaternion.LookRotation(movement.Velocity, Vector3.up);
        }

        
    }

    Vector3 Seek(GameObject target)
    {
        Vector3 direction = (target.transform.position - transform.position);
        Vector3 force = GetSteeringForce(direction);
        return force;
    }
    Vector3 Flee(GameObject target)
    {
        Vector3 direction = (transform.position - target.transform.position);
        Vector3 force = GetSteeringForce(direction);
        return force;
    }

    //Method body was generated from ChatGPT
    //https://chatgpt.com/share/696ed911-648c-800f-815a-2d51d4274a00
    Vector3 Wander()
    {
        // 1) Add a small random vector to the current wander target (jitter)
        //    Use Time.deltaTime so jitter is framerate independent.
        Vector3 randomDisplacement = new Vector3(
            Random.Range(-1f, 1f),
            0,
            Random.Range(-1f, 1f)
        ).normalized * wanderJitter * Time.deltaTime;

        wanderTarget += randomDisplacement;

        // 2) Reproject to sphere surface of radius wanderRadius
        if (wanderTarget.sqrMagnitude > 0.0001f)
            wanderTarget = wanderTarget.normalized * wanderRadius;
        else
            wanderTarget = Random.onUnitSphere * wanderRadius;

        // 3) Compute sphere center: a point ahead of the agent.
        // Prefer using movement.Direction (velocity aligned) if it's valid, otherwise fall back to transform.forward.
        Vector3 forward = movement.Velocity.sqrMagnitude > 0.0001f ? movement.Velocity.normalized : transform.forward;
        Vector3 sphereCenter = transform.position + forward * wanderDistance;

        // 4) World-space target is center + offset on sphere surface
        Vector3 targetWorld = sphereCenter + wanderTarget;

        // 5) Return steering force towards that world target
        Vector3 direction = targetWorld - transform.position;
        Vector3 force = GetSteeringForce(direction);
        force.y=0; // keep wander force horizontal
        return force;
    }

    public Vector3 GetSteeringForce(Vector3 direction)
    {
        Vector3 desired = direction.normalized * movement.maxSpeed;
        Vector3 steer = desired - movement.Velocity;
        return Vector3.ClampMagnitude(steer, movement.maxForce);
    }
}

