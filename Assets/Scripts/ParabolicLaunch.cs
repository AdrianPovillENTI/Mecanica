using UnityEngine;
using UnityEngine.UIElements;

public class ParabolicLaunch : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float totalTime;
    [SerializeField] private float dt;
    [SerializeField] private float angle;
    [SerializeField] private float speed;
    [SerializeField] private float grav;
    [SerializeField] private float constantResistance;

    private float time;

    [Header("Vectors")]
    private Vector2 position;
    private Vector2 velocity;
    private Vector2 acceleration;

    private Vector2 initialPosition = new Vector2(0, 0);

    [Header("Check collision method")]
    private Vector2 groundNormal = new Vector2(0, 1);
    [SerializeField] private float collisionFactor = 1f;


    void Start()
    {
        position = initialPosition;
        velocity = new Vector2(
            speed * Mathf.Cos(angle * Mathf.Deg2Rad), 
            speed * Mathf.Sin(angle * Mathf.Deg2Rad));
        CalculateAcceleration();
        time = 0;

        transform.position = position;
    }

    // Update is called once per frame
    void Update()
    {
        if (time < totalTime)
        {
            Vector2 newPosition, newVelocity;
            (newPosition, newVelocity) = EulerMethod(position, velocity);
            (position, velocity) = CheckGroundCollision(newPosition, position, newVelocity, velocity);
            time += dt;
            transform.position = position;
            CalculateAcceleration();
        }
    }

    (Vector2, Vector2) CheckGroundCollision(Vector2 newPos, Vector2 pos, Vector2 newVel, Vector2 vel)
    {
        float oldDot = Vector2.Dot(pos, groundNormal);
        float newDot = Vector2.Dot(newPos, groundNormal);
        
        if (oldDot * newDot <= 0)
        {
            float velocityDot = Vector2.Dot(newVel, groundNormal);
            Vector2 reflectedVel = newVel - (1 + collisionFactor) * velocityDot * groundNormal;
            
            float positionDot = Vector2.Dot(newPos, groundNormal);
            Vector2 reflectedPos = newPos - (1 + collisionFactor) * positionDot * groundNormal;

            reflectedPos += 0.01f * groundNormal;

            return (reflectedPos, reflectedVel);
        }
        return (newPos, newVel);
    }

    void CalculateAcceleration()
    {
        acceleration.x = -constantResistance * velocity.x;
        acceleration.y = - grav - constantResistance * velocity.y;
    }

    (Vector2, Vector2) EulerMethod(Vector2 position, Vector2 velocity)
    {
        Vector2 newPosition = position + velocity * dt;
        Vector2 newVelocity = velocity + acceleration * dt;

        return (newPosition, newVelocity);
    }


}
