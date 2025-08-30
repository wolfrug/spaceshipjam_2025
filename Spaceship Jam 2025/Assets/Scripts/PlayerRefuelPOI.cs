using UnityEngine;

public class PlayerRefuelPOI : PointOfInterestHUD
{
    public float refuelEfficiency = 2f;
    public override void Update()
    {
        if (scanning)
        {
            if (scanTimeLeft > 0f)
            {
                if (PlayerController.Player.ThrusterFuel < PlayerController.Player.FuelMax)
                {
                    scanTimeLeft -= Time.deltaTime;
                    animator?.SetBool("scanning", true);
                    PlayerController.Player.ThrusterFuel += Time.deltaTime * refuelEfficiency;
                }
            }
            else
            {
                scanning = false;
                animator?.SetBool("scanning", false);
                if (!finished)
                {
                    finished = true;
                    animator?.SetTrigger("finished");
                    GlobalEvents.SendOnPointOfInterestFinished(new HUDEventArgs { pointOfInterestHUD = this, success = false });
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
