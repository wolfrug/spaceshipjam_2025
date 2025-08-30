using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PointOfInterest : MonoBehaviour
{
    public PointOfInterestHUD parentPOI;
    public TextMeshProUGUI distanceText;


    public void UpdateDistanceText()
    {
        float distance = Vector2.Distance(parentPOI.transform.position, PlayerController.Player.transform.position);
        distanceText.SetText(distance.ToString("n1"));
    }
}
