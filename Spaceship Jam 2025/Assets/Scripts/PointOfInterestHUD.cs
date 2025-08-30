using UnityEngine;
using UnityEngine.Events;

public class PointOfInterestHUD : GenericWorldSpaceToCanvasIcon
{
    public GameObject hudPrefab;

    public PointOfInterest spawnedIcon;

    public CircleCollider2D scanTrigger;

    public Animator animator;

    public float scanTimeLeft = 20f;

    protected bool scanning = false;

    public bool finished
    {
        get; set;
    }

    public UnityEvent<PointOfInterestHUD> onFinished;

    public virtual void Start()
    {
        GlobalEvents.SendOnPointOfInterestSpawned(new HUDEventArgs { pointOfInterestHUD = this });
    }

    public virtual void Init(PointOfInterest newIcon)
    {
        canvasObject = newIcon.GetComponent<RectTransform>();
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            scanning = true;
        }
    }
    public virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            scanning = false;
        }
    }

    public virtual void OnDestroy()
    {
        if (!finished)
        {
            GlobalEvents.SendOnPointOfInterestFinished(new HUDEventArgs { pointOfInterestHUD = this, success = false });
        }
    }

    public virtual void Update()
    {
        if (scanning)
        {
            if (scanTimeLeft > 0f)
            {
                scanTimeLeft -= Time.deltaTime;
                animator?.SetBool("scanning", true);
            }
            else
            {
                scanning = false;
                animator?.SetBool("scanning", false);
                if (!finished)
                {
                    finished = true;
                    animator?.SetTrigger("finished");
                    GlobalEvents.SendOnPointOfInterestFinished(new HUDEventArgs { pointOfInterestHUD = this, success = true });
                    onFinished?.Invoke(this);
                }
            }
        }
        else
        {
            animator?.SetBool("scanning", false);
        }
    }
}
