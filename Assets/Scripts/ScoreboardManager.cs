using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;

public class ScoreboardManager : MonoBehaviour
{
    [SerializeField] private Transform scoreboardContainer; // Contenedor para los elementos
    [SerializeField] private GameObject scoreEntryPrefab;   // Prefab para cada entrada
    [SerializeField] private int maxEntries = 10;           // Top 10

    private const string NameListKey = "AllPlayerNames";

    private void Start()
    {
        
        if (scoreboardContainer == null)
        {
            Debug.LogError("Scoreboard container is not assigned!");
            return;
        }

        if (scoreEntryPrefab == null)
        {
            Debug.LogError("Score entry prefab is not assigned!");
            return;
        }

        LoadAndDisplayScores();
    }

    private void LoadAndDisplayScores()
    {
        List<string> playerNames = GetAllSavedNames();

        if (playerNames.Count == 0)
        {
            Debug.LogWarning("No player names found!");
            return;
        }

        foreach (string playerName in playerNames)
        {
            Debug.Log("Jugador guardado: " + playerName);  // Muestra cada nombre guardado
        }
        // Lista para almacenar las estadísticas de todos los jugadores
        List<PlayerStats> allStats = new List<PlayerStats>();

        foreach (string playerName in playerNames)
        {
            try
            {
                PlayerStats stats = LoadStatsForPlayer(playerName);
                allStats.Add(stats); // Guardamos las estadísticas del jugador

            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading stats for player {playerName}: {e.Message}");
            }
        }

        // Ordenar las estadísticas por "PartidasGanadas" (o cualquier otra métrica)
        allStats = allStats.OrderByDescending(stat => stat.PartidasGanadas).Take(maxEntries).ToList();

        // Limpiar el contenedor de scoreboard
        foreach (Transform child in scoreboardContainer)
        {
            Destroy(child.gameObject);
        }

        // Instanciar las entradas para los primeros 10 jugadores
        foreach (PlayerStats stats in allStats)
        {
            GameObject scoreEntry = Instantiate(scoreEntryPrefab, scoreboardContainer);
            ScoreEntryUI entryUI = scoreEntry.GetComponent<ScoreEntryUI>();

            if (entryUI == null)
            {
                Debug.LogError("Score entry prefab is missing ScoreEntryUI script!");
                continue;
            }

            // Configurar la entrada del scoreboard
            entryUI.SetupEntry(stats.Nombre, stats.PartidasGanadas, stats.HormigasEliminadas, stats.TermitasEliminadas, stats.ParcelasHormigas, stats.ParcelasTermitas);
        }
    }

    private List<string> GetAllSavedNames()
    {
        string rawList = PlayerPrefs.GetString("AllPlayerNames", "");
        if (string.IsNullOrEmpty(rawList))
            return new List<string>();  // No hay nombres guardados, devuelve lista vacía

        return new List<string>(rawList.Split(';'));
    }

    private PlayerStats LoadStatsForPlayer(string name)
    {
        // Si no existen estadísticas guardadas para este jugador, las inicializamos a cero
        int partidasGanadas = PlayerPrefs.GetInt($"PartidasGanadas_{name}", 0);
        int hormigasEliminadas = PlayerPrefs.GetInt($"HormigasEliminadas_{name}", 0);
        int termitasEliminadas = PlayerPrefs.GetInt($"TermitasEliminadas_{name}", 0);
        int parcelasHormigas = PlayerPrefs.GetInt($"ParcelasHormigas_{name}", 0);
        int parcelasTermitas = PlayerPrefs.GetInt($"ParcelasTermitas_{name}", 0);

        // Devolver las estadísticas
        return new PlayerStats
        {
            Nombre = name,
            PartidasGanadas = partidasGanadas,
            HormigasEliminadas = hormigasEliminadas,
            TermitasEliminadas = termitasEliminadas,
            ParcelasHormigas = parcelasHormigas,
            ParcelasTermitas = parcelasTermitas
        };
    }


}

// Clase auxiliar para guardar los datos de cada jugador
[System.Serializable]
public class PlayerStats
{
    public string Nombre;
    public int PartidasGanadas;
    public int HormigasEliminadas;
    public int TermitasEliminadas;
    public int ParcelasHormigas;
    public int ParcelasTermitas;
}
