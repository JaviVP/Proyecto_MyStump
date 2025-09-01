using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine.SceneManagement;
using TMPro;

public class BugzoniaMatchmaker : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject waitingPanel; // Panel con texto “Esperando jugador”
    [SerializeField] private TextMeshProUGUI waitingText;

    [Header("Configuración de juego")]
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private int maxPlayers = 2;
    [SerializeField] private float hostTimeout = 30f;

    private UnityTransport transport;

    private static bool hostExists = false;
    private static Allocation hostAllocation = null;
    private static JoinAllocation clientAllocation = null;

    private void Awake()
    {
        transport = FindObjectOfType<UnityTransport>();
        waitingPanel.SetActive(false);
    }

    public async void StartMatchmaking()
    {
        waitingPanel.SetActive(true);
        waitingText.text = "Buscando partida...";

        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        if (!hostExists)
        {
            hostExists = true;
            waitingText.text = "Eres el host. Esperando jugador...";

            hostAllocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            transport.SetHostRelayData(
                hostAllocation.RelayServer.IpV4,
                (ushort)hostAllocation.RelayServer.Port,
                hostAllocation.AllocationIdBytes,
                hostAllocation.Key,
                hostAllocation.ConnectionData
            );

            NetworkManager.Singleton.StartHost();
            StartCoroutine(WaitForClientAndStartGame());
        }
        else
        {
            waitingText.text = "Conectando al host...";

            clientAllocation = await RelayService.Instance.JoinAllocationAsync(hostAllocation.AllocationId.ToString());
            transport.SetClientRelayData(
                clientAllocation.RelayServer.IpV4,
                (ushort)clientAllocation.RelayServer.Port,
                clientAllocation.AllocationIdBytes,
                clientAllocation.Key,
                clientAllocation.ConnectionData,
                clientAllocation.HostConnectionData
            );

            NetworkManager.Singleton.StartClient();
        }
    }

    private IEnumerator WaitForClientAndStartGame()
    {
        float timer = 0f;

        while (NetworkManager.Singleton.ConnectedClientsList.Count < 2)
        {
            timer += Time.deltaTime;
            if (timer > hostTimeout)
            {
                waitingText.text = "No se conectó ningún jugador. Intenta de nuevo.";
                hostExists = false;
                NetworkManager.Singleton.Shutdown();
                yield break;
            }
            yield return null;
        }

        waitingText.text = "Jugador conectado. Preparando partida...";
        yield return new WaitForSeconds(1f); // Pequeña espera para suavizar transición

        waitingPanel.SetActive(false);
        NetworkManager.Singleton.SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
    }
}