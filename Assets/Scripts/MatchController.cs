using TMPro;
using UnityEngine;

public class MatchController : MonoBehaviour
{
     private int finishGame = 1;

    [SerializeField] private TextMeshProUGUI touchDebugText;
    [SerializeField] private TextMeshProUGUI touchStatusText;
    [SerializeField] private TextMeshProUGUI touchStatus;
    private void Start()
    {
        
        if (!PlayerPrefs.HasKey("FINISHGAME"))
        {
            PlayerPrefs.SetInt("FINISHGAME", finishGame);
        }
    }
    private void Update()
    {
        UpdateTouchStatus();
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            touchDebugText.text = "Touch Detected: " + touch.phase.ToString();
        }
        else
        {
            touchDebugText.text = "No Touch Detected";
        }
    }

    private void UpdateTouchStatus()
    {
        touchStatusText.text = "Touch Enabled: " + (UiButtons.Instance.TouchesEnabled() ? "ON" : "OFF");
        touchStatus.text = "Touches: " + Input.touchCount;
    }
}
