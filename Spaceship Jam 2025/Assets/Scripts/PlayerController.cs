using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Player
    {
        get
        {
            if (m_player == null)
            {
                m_player = FindFirstObjectByType<PlayerController>();
            }
            return m_player;
        }
        set
        {
            m_player = value;
        }
    }
    private static PlayerController m_player;

    public bool IsControllable
    {
        get;
        set;
    } = true;
    public Rigidbody2D playerRB;
    public float thrusterStrength = 5f;
    private float thrusterfuel;
    public float ThrusterFuel
    {
        get
        {
            return thrusterfuel;
        }
        set
        {
            thrusterfuel = Mathf.Clamp(value, -1f, FuelMax);
        }
    }

    public float FuelMax
    {
        get
        {
            return 100f;
        }
    }

    public float HP
    {
        get
        {
            return hp;
        }
        set
        {
            hp = Mathf.Clamp(value, -1f, HPMax);
        }
    }
    private float hp;
    public float HPMax
    {
        get
        {
            return 100f;
        }
    }

    public float maxVelocity = 20f;
    public Animator animator;

    public CinemachineCamera playerCamera;
    public Vector2 minMaxCameraDistance = new Vector2(2f, 10f);

    public CustomAudioSource audioSource;

    private float velocity_last_frame;
    private float min_damage_velocity = 1f;

    InputAction moveAction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Player = this;
        moveAction = InputSystem.actions.FindAction("Move");
        GlobalEvents.OnScanningStarted += GlobalEvents_ScanStarted;
        GlobalEvents.OnScanningEnded += GlobalEvents_ScanEnded;
    }

    void OnDestroy()
    {
        GlobalEvents.OnScanningStarted -= GlobalEvents_ScanStarted;
        GlobalEvents.OnScanningEnded -= GlobalEvents_ScanEnded;
    }

    void GlobalEvents_ScanStarted(HUDEventArgs args)
    {
        SetDownloading(args.pointOfInterestHUD, true);
    }
    void GlobalEvents_ScanEnded(HUDEventArgs args)
    {
        SetDownloading(args.pointOfInterestHUD, false);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        string layer = LayerMask.LayerToName(collision.gameObject.layer);
        switch (layer)
        {
            case "DangerVelocity":
                {
                    if (collision.relativeVelocity.magnitude > min_damage_velocity)
                    {
                        Debug.LogWarning("Hit velocity danger layer doing damage: " + collision.relativeVelocity.magnitude * 5f);
                        DamagePlayer(collision.relativeVelocity.magnitude * 5f);
                    }
                    break;
                }
            case "DangerTouch":
                {
                    Debug.LogWarning("Hit touch danger layer");
                    DamagePlayer(5f);
                    break;
                }
            case "InstantKill":
                {
                    Debug.LogWarning("Hit instant kill danger layer");
                    DamagePlayer(500f);
                    break;
                }
        }
    }

    public void DamagePlayer(float damage)
    {
        HP -= damage;
        GlobalEvents.SendOnPlayerTakeDamage(new PlayerEventArgs { player = this });
        audioSource.PlayRandomType(SFXType.HIT_OBJECT);
        if (HP <= 0f)
        {
            GlobalEvents.SendOnPlayerDead(new PlayerEventArgs { player = this, playerDead = true });
            animator?.SetBool("player_dead", true);
            IsControllable = false;
        }
    }

    List<PointOfInterestHUD> currentlyActiveScanners = new List<PointOfInterestHUD> { };
    public void SetDownloading(PointOfInterestHUD scannerTarget, bool isDownloading)
    {
        if (!currentlyActiveScanners.Contains(scannerTarget) && isDownloading)
        {
            currentlyActiveScanners.Add(scannerTarget);
        }
        if (currentlyActiveScanners.Contains(scannerTarget) && !isDownloading)
        {
            currentlyActiveScanners.Remove(scannerTarget);
        }
        if (currentlyActiveScanners.Count > 0)
        {
            animator?.SetBool("player_downloading", true);
            audioSource.PlayRandomType(SFXType.DOWNLOADING);
        }
        else
        {
            animator?.SetBool("player_downloading", false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Read the "Move" action value, which is a 2D vector
        // and the "Jump" action state, which is a boolean value

        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        // your movement code here
        //Debug.Log("Move value is: " + moveValue);
        if (ThrusterFuel > 0f && IsControllable)
        {
            playerRB.AddForce(moveValue * thrusterStrength);
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
            ThrusterFuel -= Mathf.Abs(moveValue.magnitude) * Time.deltaTime;
            GlobalEvents.SendOnPlayerUseThruster(new PlayerEventArgs { player = this });
        }
        else
        {
            animator.SetBool("thrust_up", false);
            animator.SetBool("thrust_down", false);
            animator.SetBool("thrust_left", false);
            animator.SetBool("thrust_right", false);
        }
        // Max player velocity. Not very realistic, but...helps.
        if (playerRB.linearVelocity.magnitude > maxVelocity)
        {
            playerRB.AddRelativeForce(-playerRB.linearVelocity);
        }

    }
    void FixedUpdate()
    {
        if (playerCamera != null && IsControllable)
        {
            playerCamera.Lens.OrthographicSize = Mathf.Lerp(playerCamera.Lens.OrthographicSize, Mathf.Clamp(playerRB.linearVelocity.magnitude, minMaxCameraDistance.x, minMaxCameraDistance.y), Time.deltaTime * 1000f);
        }
    }
    void LateUpdate()
    {
        velocity_last_frame = playerRB.linearVelocity.magnitude;
    }
}
