using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Random = UnityEngine.Random;
using TMPro;
using UnityEngine.UI;
using System.Collections;
public class HazardManager : MonoBehaviour
{
    /// Write those out ffrom here
    private int maxTurns;
    // int basicProbability = 25; //No se usa de momento lo comento
    
    private int currentProbability = 25;
    [SerializeField]
    private int maxProbability = 70;
    [SerializeField]
    private int basicAdditive = 5;
    private int currentAdditive = 5;
    [SerializeField]
    private int maxAdditive = 10;
    [SerializeField]
    private int additiveAdditive = 2;
    private int additiveUsed = 0;
    [SerializeField]
    private int startingEventHazardTurn = 2;
    [SerializeField]
    private int cooldownBetweenHazards  = 3;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    private List<Hazard> hazardPool = new List<Hazard>();

    [Header("Tier Related")]
    [SerializeField]
    private bool useTierSystem;

    [SerializeField]
    [Tooltip("Max probability for T1 and T3. T1 happens at the beginning of the game, T3 happens at the end of the game")]
    private float tier1And3MaxProbability = 0.75f;
    [SerializeField]
    [Tooltip("Max probability for T2 Happens in the middle of the game")]
    private float tier2MaxProbability = 0.6f;
    [SerializeField]
    [Tooltip("Min probability for T2. Happens in the beggining and end of the game")]
    private float tier2MinProbability = 0.2f;



    [Header("UI References")]
    public GameObject hazardPanel;
    public Image BlackFade;
    public Image eventImage;
    public Image eventBackgroundImage;
    public TextMeshProUGUI eventNameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI loreText;
    public Animator panelAnimator;

    private bool skipRequested = false;
    private bool sequenceCompleted = false;

    private Dictionary<int, Hazard> HazardByTurn = new Dictionary<int, Hazard>();

    public static HazardManager Instance { get; private set; }
    private HexGrid hexGrid;

    [SerializeField] public GameObject vfxs;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;

        }

        // DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ShuffleHazardPool();
        maxTurns = GameManager.Instance.LimitTurns;
        TurnAssignation();
        hexGrid = FindAnyObjectByType<HexGrid>();
    }
    public void ShuffleHazardPool()
    {
        System.Random rng = new System.Random();

        int n = hazardPool.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Hazard temp = hazardPool[k];
            hazardPool[k] = hazardPool[n];
            hazardPool[n] = temp;
        }
    }





    private void TurnAssignation()
    {
        int hazardPoolIndex = 0;
        int turn = startingEventHazardTurn;
        while (turn <= maxTurns){

            if (hazardPoolIndex >= hazardPool.Count)
                break;

            int rnd = Random.Range(1, 101);
            if (rnd <= currentProbability)
            {

                var hazard = hazardPool[hazardPoolIndex];
                HazardByTurn.Add(turn, hazard);
                currentAdditive = basicAdditive;
                Debug.Log($"<color=orange><b>On turn {turn}</b></color>, event <color=cyan><b>{hazard.name}</b></color> has been assigned.");
                turn += cooldownBetweenHazards + hazard.duration; // look into duration
                hazardPoolIndex++;

            }
            else
            {
                currentProbability += currentAdditive;
                additiveUsed++;
                if (currentProbability >= maxProbability)
                {
                    currentProbability = maxProbability;
                }

                if (additiveUsed >= 2)
                {
                    currentAdditive += additiveAdditive;
                    additiveUsed = 0;
                    additiveAdditive++;
                    if (currentAdditive >= maxAdditive)
                    {
                        currentAdditive = maxAdditive;
                    }
                }

                turn++;

            }

        }
        /*
        foreach (int key in HazardByTurn.Keys)
        {
            print(key + ".-"+ HazardByTurn[key]);
            
        }
        */

    }
    public void LaunchHazardUI()
    {
        
        int currentTurn = GameManager.Instance.numericCurrentTurn;

        if (HazardByTurn.TryGetValue(currentTurn, out Hazard hazard))
        {
            UiManager.Instance.TouchEnabled = false;
            Debug.Log("THERE SHOULD HAVE BEEN AN EVENT");
            hazardPanel.SetActive(true);

            // Assign content from eventData
            eventImage.sprite = hazard.eventMainImage;
            eventBackgroundImage.sprite = hazard.eventBackground;
            eventNameText.text = hazard.eventName;
            descriptionText.text = hazard.description;
            loreText.text = hazard.lore;

            // Reset alphas
            SetAlpha(BlackFade, 0f);
            SetAlpha(eventBackgroundImage, 0f);
            SetAlpha(eventNameText, 0f);
            SetAlpha(descriptionText, 0f);
            SetAlpha(loreText, 0f);

            // Assign tap handler
            hazardPanel.GetComponent<Button>().onClick.RemoveAllListeners();
            hazardPanel.GetComponent<Button>().onClick.AddListener(OnTap);

            // Start coroutine
            StartCoroutine(EventSequence());
        }
        else
        {
            Debug.Log($"<color=grey>No hazard assigned for turn {currentTurn}.</color>");
        }



    }

    private IEnumerator EventSequence()
    {
        skipRequested = false;
        sequenceCompleted = false;
        
        // Show main image instantly
        SetAlpha(eventImage, 1f);

        // Wait for panel entry + 1 sec delay


        // Fade background in
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(FadeImage(BlackFade, 0.5f));
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(FadeImage(eventBackgroundImage, 0.5f));
        

        if (skipRequested) { ShowAll(); yield break; }

        // Wait 0.5s then fade in event name
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FadeText(eventNameText, 0.3f));

        if (skipRequested) { ShowAll(); yield break; }

        // Wait 0.3s then fade in description
        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(FadeText(descriptionText, 0.3f));

        if (skipRequested) { ShowAll(); yield break; }

        // Wait 0.3s then fade in lore
        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(FadeText(loreText, 0.3f));

        if (skipRequested) { ShowAll(); yield break; }

        // Sequence completed
        sequenceCompleted = true;
        

    }

    private void OnTap()
    {
        if (!sequenceCompleted)
        {
            skipRequested = true;
        }
        else
        {
            StartCoroutine(FadeImageOut(BlackFade, 0.2f));
            panelAnimator.SetBool("Exit", true);
        }
    }

    // Helpers

    private void ShowAll()
    {
        SetAlpha(eventBackgroundImage, 1f);
        SetAlpha(eventNameText, 1f);
        SetAlpha(descriptionText, 1f);
        SetAlpha(loreText, 1f);

        sequenceCompleted = true;
    }

    private IEnumerator FadeImage(Image img, float duration)
    {
        float timer = 0f;
        Color c = img.color;
        while (timer < duration)
        {
            if (skipRequested) { SetAlpha(img, 1f); yield break; }
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, timer / duration);
            img.color = c;
            yield return null;
        }
        SetAlpha(img, 1f);
    }

    private IEnumerator FadeImageOut(Image img, float duration)
    {
        float timer = 0f;
        Color c = img.color;
        while (timer < duration)
        {
            //if (skipRequested) { SetAlpha(img, 1f); yield break; }
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, timer / duration);
            img.color = c;
            yield return null;
        }
        SetAlpha(img, 0f);
    }

    private IEnumerator FadeText(TextMeshProUGUI txt, float duration)
    {
        float timer = 0f;
        Color c = txt.color;
        while (timer < duration)
        {
            if (skipRequested) { SetAlpha(txt, 1f); yield break; }
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, timer / duration);
            txt.color = c;
            yield return null;
        }
        SetAlpha(txt, 1f);
    }

    private void SetAlpha(Graphic graphic, float alpha)
    {
        Color c = graphic.color;
        c.a = alpha;
        graphic.color = c;
    }


    public void LaunchHazard(int currentTurn)
    {



        int tier = GetWeightedRandomNumber(
            currentTurn,
            maxTurns, // max number
            tier2MaxProbability, // max prob for 2
            tier2MinProbability, // min prob for 2
            tier1And3MaxProbability // max prob for 1 and 3
        );

        if (HazardByTurn.TryGetValue(currentTurn, out Hazard hazard))
        {
            Debug.Log($"<color=red><b>Turn {currentTurn}</b></color>: Launching <color=cyan>{hazard.name}</color> hazard.");

            hazard.ExecuteHazard(useTierSystem, tier);
            if (hazard.ScreenVFX)
            {
                //Instancia
                //Instantiate(hazard.ScreenVFX, Vector3.zero, Quaternion.Euler(0,30,0));
                for(int i =0; i < vfxs.transform.childCount; i++)
                {
                   
                    if (vfxs.transform.GetChild(i).gameObject.name == hazard.ScreenVFX.name)
                    {
                     
                        vfxs.transform.GetChild(i).gameObject.SetActive(true);
                        vfxs.transform.GetChild(i).transform.position = Vector3.zero;
                        vfxs.transform.GetChild(i).transform.rotation = Quaternion.Euler(0, 30, 0);
                        break;
                    }
                }


            }
            GameManager.Instance.hazardDurationLeft = hazard.duration;
        }
        else
        {
            Debug.Log($"<color=grey>No hazard assigned for turn {currentTurn}.</color>");
        }
    }

    public void ResetTemporaryTiles()
    {
        if (hexGrid.TemporaryInactiveTiles != null)
        {
            foreach (HexTile tile in hexGrid.TemporaryInactiveTiles)
            {
                //tile.TileRenderer.gameObject.SetActive(true);
                hexGrid.hexMap.Add(tile.axialCoords, tile);
                tile.SetState(HexState.Neutral);
            }
            hexGrid.TemporaryInactiveTiles.Clear();
        }
        
    }


    public static int GetWeightedRandomNumber(
        float currentNumber,
        float maxNumber,
        float maxProbFor2,
        float minProbFor2,
        float maxProbFor1And3)
    {
        // Clamp values to avoid accidental overshooting
        currentNumber = Mathf.Clamp(currentNumber, 0f, maxNumber);

        // Normalize current number to a 0-1 range
        float t = currentNumber / maxNumber;

        // Interpolate probability for number 2 (peaks at middle)
        float prob2 = Mathf.Lerp(minProbFor2, maxProbFor2, 1f - Mathf.Abs(t - 0.5f) * 2f);

        // Remaining probability to split between 1 and 3
        float remaining = 1f - prob2;

        // Interpolate max possible values for 1 and 3 based on position
        float prob1Max = Mathf.Lerp(maxProbFor1And3, 0f, t);
        float prob3Max = Mathf.Lerp(0f, maxProbFor1And3, t);

        float totalMax = prob1Max + prob3Max;

        // Actual probabilities for 1 and 3 scaled from remaining probability
        float prob1 = (prob1Max / totalMax) * remaining;
        float prob3 = (prob3Max / totalMax) * remaining;

        // Random roll
        float rand = Random.value;

        if (rand < prob1)
            return 1;
        else if (rand < prob1 + prob2)
            return 2;
        else
            return 3;
    }

}
