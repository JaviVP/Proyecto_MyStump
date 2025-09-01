using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

public class QuickMatchmaking : MonoBehaviour
{
    [Header("UI")]
    public Button btnQuickMatch;
    public GameObject waitingPanel;

    private UnityTransport _transport;
    private const int MaxPlayers = 2;
    private static string lastHostJoinCode = null; // Simula la “cola” de matchmaking

    private async void Awake()
    {
        _transport = FindObjectOfType<UnityTransport>();
        btnQuickMatch.onClick.AddListener(StartQuickMatch);
        await Authenticate();
    }

    private static async Task Authenticate()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void StartQuickMatch()
    {
        btnQuickMatch.interactable = false;
        waitingPanel.SetActive(true);

        if (lastHostJoinCode == null)
        {
            // Soy host
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MaxPlayers);
            lastHostJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            _transport.SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            NetworkManager.Singleton.StartHost();
        }
        else
        {
            // Soy cliente y me uno al host que ya existe
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(lastHostJoinCode);

            _transport.SetClientRelayData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
            );

            NetworkManager.Singleton.StartClient();
            lastHostJoinCode = null; // Limpiar la “cola”
        }
    }
    private void OnEnable()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnDisable()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        // Solo host puede decidir cuándo cargar la escena
        if (!NetworkManager.Singleton.IsHost) return;

        // Si ya hay 2 jugadores (host + cliente)
        if (NetworkManager.Singleton.ConnectedClients.Count == MaxPlayers)
        {
            Debug.Log("Ambos jugadores conectados. Cargando escena de juego...");

            // Cambia "GameScene" por el nombre de tu escena real
            NetworkManager.Singleton.SceneManager.LoadScene("OnlineScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }
}