using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class TriggerEnteredEventArgs
{
    public GameObject triggerOwner;
    public Collider2D triggerInstigator;
}

[System.Serializable]
public class PlayerEventArgs
{
    public PlayerController player;
    public bool playerDead;
}

[System.Serializable]
public class HUDEventArgs
{
    public PointOfInterestHUD pointOfInterestHUD;
    public bool success;
}

[System.Serializable]
public class GameEventArgs
{
    public bool wonGame;
}

public static class GlobalEvents
{

    // Triggers
    public delegate void TriggeredEnteredEvent(TriggerEnteredEventArgs eventArgs);
    public static TriggeredEnteredEvent OnGravityObjectOrbitEntered;
    public static TriggeredEnteredEvent OnGravityObjectOrbitExited;

    public static TriggeredEnteredEvent OnCloseToUniverseEdgeEntered;
    public static TriggeredEnteredEvent OnCloseToUniverseEdgeExited;

    public static void SendOnGravityObjectOrbitEntered(TriggerEnteredEventArgs args)
    {
        OnGravityObjectOrbitEntered?.Invoke(args);
    }
    public static void SendOnGravityObjectOrbitExited(TriggerEnteredEventArgs args)
    {
        OnGravityObjectOrbitExited?.Invoke(args);
    }

    public static void SendOnCloseToUniverseEdgeEntered(TriggerEnteredEventArgs args)
    {
        OnCloseToUniverseEdgeEntered?.Invoke(args);
    }

    public static void SendOnCloseToUniverseEdgeExited(TriggerEnteredEventArgs args)
    {
        OnCloseToUniverseEdgeExited?.Invoke(args);
    }

    // player
    public delegate void PlayerEvent(PlayerEventArgs eventArgs);

    public static PlayerEvent OnPlayerDead;
    public static PlayerEvent OnPlayerRespawned;
    public static PlayerEvent OnPlayerUseThruster;
    public static PlayerEvent OnPlayerTakeDamage;


    public static void SendOnPlayerDead(PlayerEventArgs args)
    {
        OnPlayerDead?.Invoke(args);
    }
    public static void SendOnPlayerRespawned(PlayerEventArgs args)
    {
        OnPlayerRespawned?.Invoke(args);
    }
    public static void SendOnPlayerUseThruster(PlayerEventArgs args)
    {
        OnPlayerUseThruster?.Invoke(args);
    }
    public static void SendOnPlayerTakeDamage(PlayerEventArgs args)
    {
        OnPlayerTakeDamage?.Invoke(args);
    }

    // HUD/UI
    public delegate void HUDEvent(HUDEventArgs eventArgs);

    public static HUDEvent OnPointOfInterestSpawned;
    public static HUDEvent OnPointOfInterestFinished;
    public static void SendOnPointOfInterestSpawned(HUDEventArgs args)
    {
        OnPointOfInterestSpawned?.Invoke(args);
    }
    public static void SendOnPointOfInterestFinished(HUDEventArgs args)
    {
        OnPointOfInterestFinished?.Invoke(args);
    }

    // Game events
    public delegate void GameEvent(GameEventArgs eventArgs);
    public static GameEvent OnObjectivesComplete;
    public static void SendOnObjectivesComplete(GameEventArgs args)
    {
        OnObjectivesComplete?.Invoke(args);
    }

}
