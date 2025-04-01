using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameStatsChampionship : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI player1Text;
    [SerializeField] private TextMeshProUGUI player2Text;

    private int gamesWon1;
    private int gamesWon2;
    private int roundsWon1;
    private int roundsWon2;
    private string player1;
    private string player2;
    private int tilesAnts1;
    private int tilesAnts2;
    private int tilesTermites1;
    private int tilesTermites2;
    private int antsKilled1;
    private int antsKilled2;
    private int termitesKilled1;
    private int termitesKilled2;

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

         gamesWon1 = PlayerPrefs.GetInt($"PartidasGanadas_{player1}", 0);
         gamesWon2 = PlayerPrefs.GetInt($"PartidasGanadas_{player2}", 0);
        
         roundsWon1 = PlayerPrefs.GetInt($"RondasGanadas_{player1}", 0);
         roundsWon2 = PlayerPrefs.GetInt($"RondasGanadas_{player2}", 0);

         tilesAnts1 = PlayerPrefs.GetInt($"ParcelasHormigas_{player1}", 0);
         tilesAnts2 = PlayerPrefs.GetInt($"ParcelasHormigas_{player2}", 0);

         tilesTermites1 = PlayerPrefs.GetInt($"ParcelasTermitas_{player1}", 0);
         tilesTermites2 = PlayerPrefs.GetInt($"ParcelasTermitas_{player2}", 0);

         antsKilled1 = PlayerPrefs.GetInt($"HormigasEliminadas_{player1}", 0);
         antsKilled2 = PlayerPrefs.GetInt($"HormigasEliminadas_{player2}", 0);

         termitesKilled1 = PlayerPrefs.GetInt($"TermitasEliminadas_{player1}", 0);
         termitesKilled2 = PlayerPrefs.GetInt($"TermitasEliminadas_{player2}", 0);

        player1Text.text = player1;
        player2Text.text = player2;
    }

    public void SetPlayerStats(string playerName, int gamesWon, int roundsWon, int tilesAnts, int tilesTermites, int antsKilled, int termitesKilled)
    {
        PlayerPrefs.SetInt($"PartidasGanadas_{playerName}", gamesWon);
        PlayerPrefs.SetInt($"RondasGanadas_{playerName}", roundsWon);
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

        SceneManager.LoadScene(5);
    }

   
}
