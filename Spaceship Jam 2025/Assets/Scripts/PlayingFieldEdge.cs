using UnityEngine;

public class PlayingFieldEdge : MonoBehaviour
{
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            GlobalEvents.SendOnCloseToUniverseEdgeEntered(new TriggerEnteredEventArgs { triggerOwner = gameObject, triggerInstigator = collision });
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            GlobalEvents.SendOnCloseToUniverseEdgeExited(new TriggerEnteredEventArgs { triggerOwner = gameObject, triggerInstigator = collision });
        }
    }
}
