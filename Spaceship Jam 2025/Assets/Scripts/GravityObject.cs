using UnityEngine;

public class GravityObject : MonoBehaviour
{
    public static float gravityModifier = 0.15f;
    public Rigidbody2D self;
    public CircleCollider2D gravityWell;
    public float gravityStrength = 5f;

    public Vector2 startingVelocity = Vector2.zero;

    public bool automatic_gravity_calculation = true;


    public void OnTriggerStay2D(Collider2D collision)
    {
        Rigidbody2D rb = collision.attachedRigidbody;
        if (rb != null)
        {
            Vector2 direction = transform.position - rb.transform.position;
            direction = direction.normalized;
            float distance = Vector2.Distance(transform.position, rb.transform.position);
            rb.AddRelativeForce(direction * gravityStrength/distance);
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (automatic_gravity_calculation && self != null)
        {
            gravityStrength = self.mass * gravityModifier;
            gravityWell.radius = gravityStrength*1.5f;
        }
        self.linearVelocity = startingVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
