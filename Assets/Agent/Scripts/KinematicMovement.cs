using UnityEngine;

public class KinematicMovement : Movement
{
    public override void ApplyForce(Vector3 force)
    {
        // Newton's second law is F=ma
        // F: Force
        // a: Acceleration
        // m: Mass
        // F=ma -> a=F/m 
        // a = F (mass is 1)

        // accumulate forces during the frame (for example: engine force + gravity)
        Acceleration += force;
    }

    private void LateUpdate()
    {
        // integrate acceleration into velocity
        Velocity += Acceleration * Time.deltaTime;
        // clamp the length (magnitude) of the velocity to max speed
        Velocity = Vector3.ClampMagnitude(Velocity, maxSpeed);

        // integrate velocity into position
        transform.position += Velocity * Time.deltaTime;
        // reset acceleration to zero for next frame, forces are recomputed each frame
        Acceleration = Vector3.zero;
    }
}

