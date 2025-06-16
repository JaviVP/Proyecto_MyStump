using UnityEngine;
using UnityEngine.UI;

public class ToggleEvents : MonoBehaviour
{
    private Toggle toggle;
    private const string eventsKey = "EventsOn";

    void Start()
    {
        toggle = GetComponent<Toggle>();

        // Load the saved value (ensure key matches GameManager)
        int useEvents = PlayerPrefs.GetInt(eventsKey, 0);
        toggle.isOn = useEvents == 1;
        toggle.onValueChanged.AddListener(Toggle);
        // Subscribe to toggle change event
        //toggle.onValueChanged.AddListener(GameManager.Instance.ToggleEvents);

    }

    public void Toggle(bool isOn)
    {
        PlayerPrefs.SetInt("EventsOn", isOn ? 1 : 0);
        PlayerPrefs.Save();
    }
}
