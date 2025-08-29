using UnityEngine;

public class GravityObject : MonoBehaviour
{
    public Collider2D gravityWell;
    public float gravityStrength = 5f;


    public void OnTriggerStay2D(Collider2D collision)
    {
        Rigidbody2D rb = collision.attachedRigidbody;
        if (rb != null)
        {
            Vector2 direction = transform.position - rb.transform.position;
            direction = direction.normalized;
            rb.AddRelativeForce(direction * gravityStrength);
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
