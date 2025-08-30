using UnityEngine;

public class GravityObject : MonoBehaviour
{
    public static float gravityModifier = 0.15f;
    public Rigidbody2D self;
    public CircleCollider2D gravityWell;
    public float gravityStrength = 5f;
    public Vector2 startingVelocity = Vector2.zero;
    public bool automatic_gravity_calculation = true;
    public bool random_starting_velocity = true;


    public void OnTriggerStay2D(Collider2D collision)
    {
        Rigidbody2D rb = collision.attachedRigidbody;
        if (rb != null)
        {
            Vector2 direction = transform.position - rb.transform.position;
            direction = direction.normalized;
            float distance = Vector2.Distance(transform.position, rb.transform.position);
            rb.AddRelativeForce(direction * Mathf.Sqrt(gravityStrength / distance), ForceMode2D.Force);
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
       // Debug.Log("Trigger entered gravity object: " + collision.gameObject.name, gameObject);
        GlobalEvents.SendOnGravityObjectOrbitEntered(new TriggerEnteredEventArgs { triggerInstigator = collision, triggerOwner = gameObject });
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
      //  Debug.Log("Trigger exited gravity object: " + collision.gameObject.name, gameObject);
        GlobalEvents.SendOnGravityObjectOrbitExited(new TriggerEnteredEventArgs { triggerInstigator = collision, triggerOwner = gameObject });
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (automatic_gravity_calculation && self != null)
        {
            gravityStrength = self.mass * gravityModifier;
            gravityWell.radius = gravityStrength * 1.5f;
        }
        if (random_starting_velocity)
        {
            startingVelocity = new Vector2(Random.Range(startingVelocity.x, startingVelocity.y), Random.Range(startingVelocity.x, startingVelocity.y));
        }
        self.linearVelocity = startingVelocity;
    }
}
