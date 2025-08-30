using Unity.Cinemachine;
using UnityEngine;

public class OrbitCameraController : MonoBehaviour
{
    public CinemachineTargetGroup targetGroup;

    public float defaultWeight = 1f;
    public float defaultRadius = 0.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GlobalEvents.OnGravityObjectOrbitEntered += GlobalEventOnOrbitEntered;
        GlobalEvents.OnGravityObjectOrbitExited += GlobalEventOnOrbitExited;
    }
    void OnDestroy()
    {
        GlobalEvents.OnGravityObjectOrbitEntered -= GlobalEventOnOrbitEntered;
        GlobalEvents.OnGravityObjectOrbitExited -= GlobalEventOnOrbitExited;
    }

    void GlobalEventOnOrbitEntered(TriggerEnteredEventArgs args)
    {
        if (args.triggerInstigator.gameObject == PlayerController.Player)
        {
            if (targetGroup.FindMember(args.triggerOwner.transform) < 0)
            {
                targetGroup.AddMember(args.triggerOwner.transform, defaultWeight, defaultRadius);
            }
        }
    }
    void GlobalEventOnOrbitExited(TriggerEnteredEventArgs args)
    {
        if (args.triggerInstigator.gameObject == PlayerController.Player)
        {
            targetGroup.RemoveMember(args.triggerOwner.transform);
        }
    }
}
