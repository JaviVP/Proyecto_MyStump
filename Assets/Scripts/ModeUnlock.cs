using UnityEngine;
using TMPro;
using System.Collections;

public class ModeUnlock : MonoBehaviour
{
    [SerializeField] private GameObject textUnlock;
    [SerializeField] private GameObject panelUnlock;

    private void Start()
    {
        if(PlayerPrefs.GetInt("FINISHGAME") == 0)
        {
            panelUnlock.SetActive(false);
        }
    }
    public void UnlockMode()
    {
        if (PlayerPrefs.GetInt("FINISHGAME") == 1)
        {
            textUnlock.SetActive(true);
           
            StartCoroutine(DeactivateTextAfterDelay(2.5f));
        }
        else
        {
            panelUnlock.SetActive(false);
        }
    }

    

  
    private IEnumerator DeactivateTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);  

        textUnlock.SetActive(false);  
    }
}
