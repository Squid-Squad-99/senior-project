using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using dotenv.net;
using SocketIOClient;


/// <summary>
/// socket to match making server & relay server
/// try to connect to server when load
/// </summary>
public class MatchNRelaySocket : MonoBehaviour
{
    public event Action ConnectedEvent; // emit when socket is readied
    public event Action<TicketPck> GetTicketPckEvent;
    public event Action<SocketIOResponse> PeerSocketRecvEvent;
    public bool IsConnected => _socket.Connected;
    public static MatchNRelaySocket Singleton { get; private set; }
    
    private SocketIO _socket;

    public async Task SendPlayerData(PlayerData playerData)
    {
        Debug.Assert(IsConnected);
        PlayerDataPck playerDataPck = new PlayerDataPck(playerData.id);
        await _socket.EmitAsync(PacketNames.PlayerData, playerDataPck);
    }

    public async Task RequestMatch()
    {
        Debug.Assert(IsConnected);
        await _socket.EmitAsync(PacketNames.RequestMatch, new RequestMatchPck());
    }

    public async Task PeerSocketSend(GameDataPck gameDataPck)
    {
        await _socket.EmitAsync(PacketNames.GameData, gameDataPck);
    }

    public async Task CancelMatch()
    {
        Debug.Assert(IsConnected);
        await _socket.EmitAsync(PacketNames.CancelMatch, "");
    }
    
    private async void Awake()
    {
        // singleton
        if (Singleton == null) Singleton = this;
        // create socket
        (string host, string port) = GetHostNPort();
        _socket = new SocketIO($"http://{host}:{port}");
        
        // emit event when readied(connected to server)
        _socket.OnConnected += (sender, args) =>
        {
            print("connect to server");
            _socket.On(PacketNames.Ticket, (res) =>
            {
                TicketPck ticketPck = res.GetValue<TicketPck>();
                GetTicketPckEvent?.Invoke(ticketPck);
            });
            _socket.On(PacketNames.GameData, (res) =>
            {
                PeerSocketRecvEvent?.Invoke(res);
            });
            ConnectedEvent?.Invoke();
        };
        
        // connect to server
        await _socket.ConnectAsync();
    }

    private async void OnDestroy()
    {
        if (_socket != null)
        {
            await _socket.DisconnectAsync();
        }
    }

    // return host, port
    private Tuple<string, string> GetHostNPort()
    {
        DotEnv.Load();
        var envVars = DotEnv.Read();
        string host, port;
        if (!envVars.TryGetValue("HOST", out host) || !envVars.TryGetValue("PORT", out port))
        {
            throw new ArgumentException(".env not found or not have host, port value");
        }

        return new Tuple<string, string>(host, port);
    }
}