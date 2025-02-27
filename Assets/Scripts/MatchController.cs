using UnityEngine;

public class MatchController : MonoBehaviour
{
     private int finishGame = 1;



    private void Start()
    {
        if (!PlayerPrefs.HasKey("FINISHGAME"))
        {
            PlayerPrefs.SetInt("FINISHGAME", finishGame);
        }
    }
}
