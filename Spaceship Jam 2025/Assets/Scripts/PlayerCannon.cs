using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    public Transform playerPosition;

    public Animator animator;

    public PointOfInterestHUD selfMarker;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        fireAction = InputSystem.actions.FindAction("Jump");
        currentAimAngle = transform.localRotation.eulerAngles;
        LoadPlayerIntoCannon();
        GlobalEvents.OnPlayerDead += GlobalEvents_OnPlayerDead;
        GlobalEvents.OnObjectivesComplete += GlobalEvents_OnObjectiveComplete;
        selfMarker.onFinished.AddListener((x) => WonGame());
    }

    void OnDestroy()
    {
        GlobalEvents.OnPlayerDead -= GlobalEvents_OnPlayerDead;
    }

    void GlobalEvents_OnPlayerDead(PlayerEventArgs args)
    {
        LoadPlayerIntoCannon();
    }

    void GlobalEvents_OnObjectiveComplete(GameEventArgs args)
    {
        selfMarker.gameObject.SetActive(true);
    }

    void WonGame()
    {
        SceneManager.LoadScene("EndScene");
    }

    public void LoadPlayerIntoCannon()
    {
        if (PlayerController.Player != null)
        {
            loadedTarget = PlayerController.Player;
            loadedTarget.HP = loadedTarget.HPMax;
            loadedTarget.ThrusterFuel = loadedTarget.FuelMax;
            loadedTarget.transform.SetParent(playerPosition);
            loadedTarget.playerRB.bodyType = RigidbodyType2D.Kinematic;
            loadedTarget.playerRB.linearVelocity = Vector2.zero;
            loadedTarget.transform.localPosition = Vector3.zero;
            loadedTarget.IsControllable = false;
            loadedTarget.playerCamera.Lens.OrthographicSize = lensOrthoSize;
            selfMarker.gameObject.SetActive(false);
        }
    }
    public void FirePlayerFromCannon()
    {
        if (loadedTarget != null)
        {
            animator.SetTrigger("launch");
            loadedTarget.playerRB.bodyType = RigidbodyType2D.Dynamic;
            loadedTarget.playerRB.AddForce(cannonBarrel.transform.right * fireForce, ForceMode2D.Impulse);
            loadedTarget.IsControllable = true;
            loadedTarget.transform.parent = null;
            loadedTarget.transform.eulerAngles = Vector3.zero;
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

            cannonBarrel.transform.Rotate(0f, 0f, -moveValue.x * Time.deltaTime * rotateSpeed);
            cannonBarrel.transform.localEulerAngles = new Vector3(cannonBarrel.transform.localEulerAngles.x, cannonBarrel.transform.localEulerAngles.y, Mathf.Clamp(cannonBarrel.transform.localEulerAngles.z, minMaxAimAngle.x, minMaxAimAngle.y));
            currentAimAngle = cannonBarrel.transform.localEulerAngles;

            if (fireAction.IsPressed())
            {
                FirePlayerFromCannon();
            }
        }
    }
}
