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

    public TextMeshProUGUI objectiveText;

    public GameObject outOfRangeObject;

    public GameObject probeLostObject;

    public GameObject downloadingObject;
    public Animator downloadingAnimator;

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
        GlobalEvents.OnPlayerRespawned += GlobalEvents_OnPlayerRespawned;
        GlobalEvents.OnPointOfInterestSpawned += GlobalEvents_OnNewPOISpawned;
        GlobalEvents.OnPointOfInterestFinished += GlobalEvents_OnPOIFinished;
        GlobalEvents.OnCloseToUniverseEdgeEntered += GlobalEvents_OnEdgeEntered;
        GlobalEvents.OnCloseToUniverseEdgeExited += GlobalEvents_OnEdgeExited;
        GlobalEvents.OnScanningStarted += GlobalEvents_ScanStarted;
        GlobalEvents.OnScanningEnded += GlobalEvents_ScanEnded;

        victoryCounterText.SetText(string.Format("{0}/{1}", currentVictoryPoints, maxVictoryPoints));
        objectiveText.SetText("Gather " + maxVictoryPoints + " data cores from nearby asteroids.");
    }

    void OnDestroy()
    {
        GlobalEvents.OnPlayerTakeDamage -= GlobalEvents_OnPlayerTakeDamage;
        GlobalEvents.OnPlayerUseThruster -= GlobalEvents_OnPlayerUseThruster;
        GlobalEvents.OnPlayerDead -= GlobalEvents_OnPlayerDead;
        GlobalEvents.OnPlayerRespawned -= GlobalEvents_OnPlayerRespawned;
        GlobalEvents.OnPointOfInterestSpawned -= GlobalEvents_OnNewPOISpawned;
        GlobalEvents.OnPointOfInterestFinished -= GlobalEvents_OnPOIFinished;
        GlobalEvents.OnCloseToUniverseEdgeEntered -= GlobalEvents_OnEdgeEntered;
        GlobalEvents.OnCloseToUniverseEdgeExited -= GlobalEvents_OnEdgeExited;
        GlobalEvents.OnScanningStarted -= GlobalEvents_ScanStarted;
        GlobalEvents.OnScanningEnded -= GlobalEvents_ScanEnded;
    }

    void ResetHUD()
    {
        healthImage.fillAmount = 1f;
        fuelImage.fillAmount = 1f;
        currentVictoryPoints = 0;
        victoryCounterText.SetText(string.Format("{0}/{1}", currentVictoryPoints, maxVictoryPoints));
        objectiveText.SetText("Gather " + maxVictoryPoints + " data cores from nearby asteroids.");
    }

    void CountUpSuccess()
    {

        currentVictoryPoints++;
        victoryCounterText.SetText(string.Format("{0}/{1}", currentVictoryPoints, maxVictoryPoints));
        Debug.Log("Counting up success! Current victory points: " + currentVictoryPoints);
        if (currentVictoryPoints >= maxVictoryPoints)
        {
            GlobalEvents.SendOnObjectivesComplete(new GameEventArgs { wonGame = false });
            objectiveText.SetText("Return to base with the data!");
        }
    }

    List<PointOfInterestHUD> currentlyActiveScanners = new List<PointOfInterestHUD> { };
    public void SetDownloading(HUDEventArgs args, bool isDownloading)
    {
        PointOfInterestHUD scannerTarget = args.pointOfInterestHUD;
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
            downloadingObject.SetActive(true);
            Debug.LogWarning("Setting downloading completion to " + (1f - (args.TimeLeft / args.ScanTimeMax)) + " from max scan time " + args.ScanTimeMax + " and current scan time " + args.TimeLeft);
            downloadingAnimator.SetFloat("completion", 1f-(args.TimeLeft / args.ScanTimeMax));
        }
        else
        {
            downloadingObject.SetActive(false);
        }
    }


    void GlobalEvents_ScanStarted(HUDEventArgs args)
    {
        SetDownloading(args, true);

    }
    void GlobalEvents_ScanEnded(HUDEventArgs args)
    {
        SetDownloading(args, false);
    }

    void GlobalEvents_OnEdgeEntered(TriggerEnteredEventArgs args)
    {
        outOfRangeObject.SetActive(true);
    }
    void GlobalEvents_OnEdgeExited(TriggerEnteredEventArgs args)
    {
        outOfRangeObject.SetActive(false);
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
        outOfRangeObject.SetActive(false);
        probeLostObject.SetActive(true);
    }
    void GlobalEvents_OnPlayerRespawned(PlayerEventArgs args)
    {
        ResetHUD();
        probeLostObject.SetActive(false);
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
