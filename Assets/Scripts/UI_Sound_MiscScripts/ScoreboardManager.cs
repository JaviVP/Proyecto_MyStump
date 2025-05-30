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
    [SerializeField] private int maxEntries = 10;
    [SerializeField] private List<ScoreEntryUI> fixedEntries = new List<ScoreEntryUI>();

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
            Debug.Log("Jugador guardado: " + playerName);  // Mostrar cada nombre guardado
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

        // Ordenar por partidas ganadas y limitar al top 10
        allStats = allStats.OrderByDescending(stat => stat.PartidasGanadas).Take(maxEntries).ToList();

        // Limpiar el contenedor de scoreboard
        for (int i = 0; i < fixedEntries.Count; i++)
        {
            if (i < allStats.Count)
            {
                PlayerStats stats = allStats[i];
                int score = CalcularScore(stats);
                fixedEntries[i].gameObject.SetActive(true);
                fixedEntries[i].SetupEntry(i + 1, stats.Nombre, stats.PartidasGanadas, score);
            }
            else
            {
                fixedEntries[i].gameObject.SetActive(false); // Ocultar si no hay tantos jugadores
            }
        }

        // Instanciar las entradas con número de ranking
       /* for (int i = 0; i < allStats.Count; i++)
        {
            PlayerStats stats = allStats[i];
            int rank = i + 1; // Top 1, Top 2...

            GameObject scoreEntry = Instantiate(scoreEntryPrefab, scoreboardContainer);
            ScoreEntryUI entryUI = scoreEntry.GetComponent<ScoreEntryUI>();

            if (entryUI == null)
            {
                Debug.LogError("Score entry prefab is missing ScoreEntryUI script!");
                continue;
            }

            // Ahora incluye el número de ranking
            int score = CalcularScore(stats);
            entryUI.SetupEntry(rank, stats.Nombre, stats.PartidasGanadas, score);
        }*/
    }
    private int CalcularScore(PlayerStats stats)
    {
        int puntosPartidas = stats.PartidasGanadas * 100;
        int puntosParcelas = (stats.ParcelasHormigas + stats.ParcelasTermitas) * 5;
        int puntosUnidades = (stats.HormigasEliminadas + stats.TermitasEliminadas) * 10;

        return puntosPartidas + puntosParcelas + puntosUnidades;
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
        int partidasGanadas = PlayerPrefs.GetInt($"PartidasGanadas_{name}");
        int hormigasEliminadas = PlayerPrefs.GetInt($"HormigasEliminadas_{name}");
        int termitasEliminadas = PlayerPrefs.GetInt($"TermitasEliminadas_{name}");
        int parcelasHormigas = PlayerPrefs.GetInt($"ParcelasHormigas_{name}");
        int parcelasTermitas = PlayerPrefs.GetInt($"ParcelasTermitas_{name}");

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