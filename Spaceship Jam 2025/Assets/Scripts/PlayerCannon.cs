using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCannon : MonoBehaviour
{
    InputAction moveAction;
    InputAction fireAction;

    public Vector2 currentAimAngle;

    public float rotateSpeed = 45f;

    public Vector2 minMaxAimAngle = new Vector2(-90f, 90f);

    public float fireForce = 5f;

    public float lensOrthoSize = 20f;

    public PlayerController loadedTarget;
    public Transform cannonBarrel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        fireAction = InputSystem.actions.FindAction("Attack");
        currentAimAngle = transform.localRotation.eulerAngles;
        LoadPlayerIntoCannon();
    }

    public void LoadPlayerIntoCannon()
    {
        if (PlayerController.Player != null)
        {
            loadedTarget = PlayerController.Player;
            loadedTarget.transform.SetParent(cannonBarrel);
            loadedTarget.playerRB.bodyType = RigidbodyType2D.Kinematic;
            loadedTarget.transform.localPosition = Vector3.zero;
            loadedTarget.IsControllable = false;
            loadedTarget.playerCamera.Lens.OrthographicSize = lensOrthoSize;
        }
    }
    public void FirePlayerFromCannon()
    {
        if (loadedTarget != null)
        {
            loadedTarget.playerRB.bodyType = RigidbodyType2D.Dynamic;
            loadedTarget.playerRB.AddForce(transform.right * fireForce, ForceMode2D.Impulse);
            loadedTarget.IsControllable = true;
            loadedTarget.transform.parent = null;
            loadedTarget = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Read the "Move" action value, which is a 2D vector
        // and the "Jump" action state, which is a boolean value
        if (loadedTarget != null)
        {
            Vector2 moveValue = moveAction.ReadValue<Vector2>();

            transform.Rotate(0f, 0f, -moveValue.x * Time.deltaTime * rotateSpeed);
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, Mathf.Clamp(transform.localEulerAngles.z, minMaxAimAngle.x, minMaxAimAngle.y));
            currentAimAngle = transform.localEulerAngles;

            if (fireAction.IsPressed())
            {
                FirePlayerFromCannon();
            }
        }
    }
}
