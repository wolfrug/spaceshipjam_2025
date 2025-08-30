using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class TriggerEnteredEventArgs
{
    public GameObject triggerOwner;
    public Collider2D triggerInstigator;
}

public static class GlobalEvents
{

    // Triggers
    public delegate void TriggeredEnteredEvent(TriggerEnteredEventArgs eventArgs);
    public static TriggeredEnteredEvent OnGravityObjectOrbitEntered;
    public static TriggeredEnteredEvent OnGravityObjectOrbitExited;

    public static void SendOnGravityObjectOrbitEntered(TriggerEnteredEventArgs args)
    {
        OnGravityObjectOrbitEntered?.Invoke(args);
    }
    public static void SendOnGravityObjectOrbitExited(TriggerEnteredEventArgs args)
    {
        OnGravityObjectOrbitExited?.Invoke(args);
    }
}
