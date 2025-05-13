using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameStatsChampionship : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI player1Text;
    [SerializeField] private TextMeshProUGUI player2Text;
    private string player1;
    private string player2;


    public static GameStatsChampionship Instance { get; private set; }
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

    void Start()
    {
         player1 = PlayerPrefs.GetString("PlayerName1", "Jugador 1");
         player2 = PlayerPrefs.GetString("PlayerName2", "Jugador 2");


        player1Text.text = player1;
        player2Text.text = player2;
    }

    public void SetPlayerStats(string playerName, int gamesWon, int tilesAnts, int tilesTermites, int antsKilled, int termitesKilled)
    {
        PlayerPrefs.SetInt($"PartidasGanadas_{playerName}", gamesWon);
        PlayerPrefs.SetInt($"ParcelasHormigas_{playerName}", tilesAnts);
        PlayerPrefs.SetInt($"ParcelasTermitas_{playerName}", tilesTermites);
        PlayerPrefs.SetInt($"HormigasEliminadas_{playerName}", antsKilled);
        PlayerPrefs.SetInt($"TermitasEliminadas_{playerName}", termitesKilled);
        PlayerPrefs.Save();
    }

    public int GetPlayerStat(string key)
    {
        return PlayerPrefs.GetInt(key, 0);
    }

    public void NextRoundChampionship()
    { 

        SceneManager.LoadScene(4);
    }

   
}
