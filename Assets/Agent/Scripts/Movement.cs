using UnityEngine;

public abstract class Movement : MonoBehaviour
{
    public float maxSpeed = 1.0f;
    public float maxForce = 1.0f;

    public virtual Vector3 Velocity { get; set; }
    public virtual Vector3 Acceleration { get; set; }
    public virtual Vector3 Direction { get { return Velocity.normalized; } }
    

    public abstract void ApplyForce(Vector3 force);
}

