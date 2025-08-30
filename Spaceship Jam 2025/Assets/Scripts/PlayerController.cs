using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D playerRB;
    public float thrusterStrength = 5f;

    public float maxVelocity = 20f;
    public Animator animator;

    public CinemachineCamera playerCamera;
    public Vector2 minMaxCameraDistance = new Vector2(2f, 10f);

    InputAction moveAction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
    }

    // Update is called once per frame
    void Update()
    {
        // Read the "Move" action value, which is a 2D vector
        // and the "Jump" action state, which is a boolean value

        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        // your movement code here
        Debug.Log("Move value is: " + moveValue);
        playerRB.AddRelativeForce(moveValue * thrusterStrength);
        if (animator != null)
        {
            if (moveValue.x > 0)
            {
                animator.SetBool("thrust_left", true);
            }
            if (moveValue.x < 0)
            {
                animator.SetBool("thrust_right", true);
            }
            if (moveValue.x == 0f)
            {
                animator.SetBool("thrust_left", false);
                animator.SetBool("thrust_right", false);
            }
            if (moveValue.y < 0)
            {
                animator.SetBool("thrust_up", true);
            }
            if (moveValue.y > 0)
            {
                animator.SetBool("thrust_down", true);
            }
            if (moveValue.y == 0f)
            {
                animator.SetBool("thrust_up", false);
                animator.SetBool("thrust_down", false);
            }
        }
        if (playerRB.linearVelocity.magnitude > maxVelocity)
        {
            playerRB.AddRelativeForce(-playerRB.linearVelocity);
        }

    }
    void FixedUpdate()
    {
        if (playerCamera != null)
        {
            playerCamera.Lens.OrthographicSize = Mathf.Lerp(playerCamera.Lens.OrthographicSize, Mathf.Clamp(playerRB.linearVelocity.magnitude, minMaxCameraDistance.x, minMaxCameraDistance.y), Time.deltaTime);
        }
    }
}
