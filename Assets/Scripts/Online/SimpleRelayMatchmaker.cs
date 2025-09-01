using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleRelayMatchmaker : MonoBehaviour
{
    [SerializeField] private bool isHost = false; // Marca si este jugador será host
    [SerializeField] private string joinCodeInput = ""; // Para cliente
    [SerializeField] private string gameSceneName = "GameScene"; // Escena a cargar

    private UnityTransport transport;

    private async void Start()
    {
        transport = FindObjectOfType<UnityTransport>();
        await InitializeServices();

        if (isHost)
        {
            await HostGame();
        }
        else
        {
            await JoinGame(joinCodeInput);
        }
    }

    private async Task InitializeServices()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        Debug.Log($"PlayerId: {AuthenticationService.Instance.PlayerId} authenticated!");
    }

    private async Task HostGame()
    {
        try
        {
            const int maxPlayers = 2;
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log($"[HOST] Game created! Join code: {joinCode}");

            // Configurar UnityTransport como host
            transport.SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            // Iniciar host
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
        catch (Exception e)
        {
            Debug.LogError($"[HOST] Failed to create Relay: {e}");
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"[HOST] Client connected: {clientId}. Loading game scene...");
        SceneManager.LoadScene(gameSceneName);
    }

    private async Task JoinGame(string joinCode)
    {
        try
        {
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            Debug.Log("[CLIENT] Joined Relay!");

            transport.SetClientRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData,
                allocation.HostConnectionData
            );

            NetworkManager.Singleton.StartClient();
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedClient;
        }
        catch (Exception e)
        {
            Debug.LogError($"[CLIENT] Failed to join Relay: {e}");
        }
    }

    private void OnClientConnectedClient(ulong clientId)
    {
        Debug.Log("[CLIENT] Connected to host! Loading game scene...");
        SceneManager.LoadScene(gameSceneName);
    }
}