using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    public Image healthImage;
    public Image fuelImage;
    public TextMeshProUGUI victoryCounterText;
    public RectTransform pointsOfInterest;

    public int currentVictoryPoints = 0;
    public int maxVictoryPoints = 10;

    public List<PointOfInterest> activePOIs = new List<PointOfInterest> { };

    public void AddNewObjectiveIcon(PointOfInterestHUD newIcon)
    {
        if (newIcon.hudPrefab != null)
        {
            GameObject newPOIGO = Instantiate(newIcon.hudPrefab, pointsOfInterest);
            PointOfInterest newPOI = newPOIGO.GetComponent<PointOfInterest>();
            newPOI.parentPOI = newIcon;
            newIcon.Init(newPOI);
            activePOIs.Add(newPOI);
        }
    }
    public void RemoveObjectiveIcon(PointOfInterestHUD targetIcon)
    {
        PointOfInterest target = activePOIs.Find((x) => x.parentPOI == targetIcon);
        activePOIs.Remove(target);
        Destroy(target.gameObject);
    }

    void Awake()
    {
        GlobalEvents.OnPlayerTakeDamage += GlobalEvents_OnPlayerTakeDamage;
        GlobalEvents.OnPlayerUseThruster += GlobalEvents_OnPlayerUseThruster;
        GlobalEvents.OnPlayerDead += GlobalEvents_OnPlayerDead;
        GlobalEvents.OnPointOfInterestSpawned += GlobalEvents_OnNewPOISpawned;
        GlobalEvents.OnPointOfInterestFinished += GlobalEvents_OnPOIFinished;

        victoryCounterText.SetText(string.Format("{0}/{1}", currentVictoryPoints, maxVictoryPoints));
    }

    void OnDestroy()
    {
        GlobalEvents.OnPlayerTakeDamage -= GlobalEvents_OnPlayerTakeDamage;
        GlobalEvents.OnPlayerUseThruster -= GlobalEvents_OnPlayerUseThruster;
        GlobalEvents.OnPlayerDead -= GlobalEvents_OnPlayerDead;
        GlobalEvents.OnPointOfInterestSpawned -= GlobalEvents_OnNewPOISpawned;
        GlobalEvents.OnPointOfInterestFinished -= GlobalEvents_OnPOIFinished;
    }

    void ResetHUD()
    {
        healthImage.fillAmount = 1f;
        fuelImage.fillAmount = 1f;
    }

    void CountUpSuccess()
    {

        currentVictoryPoints++;
        victoryCounterText.SetText(string.Format("{0}/{1}", currentVictoryPoints, maxVictoryPoints));
        Debug.Log("Counting up success! Current victory points: " + currentVictoryPoints);
        if (currentVictoryPoints >= maxVictoryPoints)
        {
            SceneManager.LoadScene("EndScene");
        }
    }

    void GlobalEvents_OnNewPOISpawned(HUDEventArgs args)
    {
        AddNewObjectiveIcon(args.pointOfInterestHUD);
    }

    void GlobalEvents_OnPOIFinished(HUDEventArgs args)
    {
        Debug.Log("Received POI finished event from " + args.pointOfInterestHUD + " as a result of success: " + args.success);
        RemoveObjectiveIcon(args.pointOfInterestHUD);
        if (args.success)
        {
            CountUpSuccess();
        }
    }

    void GlobalEvents_OnPlayerDead(PlayerEventArgs args)
    {
        ResetHUD();
    }

    void GlobalEvents_OnPlayerTakeDamage(PlayerEventArgs args)
    {
        healthImage.fillAmount = args.player.HP / args.player.HPMax;
    }
    void GlobalEvents_OnPlayerUseThruster(PlayerEventArgs args)
    {
        fuelImage.fillAmount = args.player.ThrusterFuel / args.player.FuelMax;
    }

    void Update()
    {
        foreach (PointOfInterest poi in activePOIs)
        {
            poi.UpdateDistanceText();
        }
    }
}
