using System;
using System.Collections;
using System.Threading.Tasks;
using dotenv.net;
using SocketIOClient;
using UnityEngine;


/// <summary>
/// socket to match making server & relay server
/// try to connect to server when load
/// </summary>
public class MatchNRelaySocket : MonoBehaviour
{
    public event Action ConnectedEvent; // emit when socket is readied
    public event Action<TicketPck> GetTicketPckEvent;
    public event Action<GameDataPck> PeerRecvEvent;
    public bool IsConnected => _socket.Connected;
    public static MatchNRelaySocket Singleton { get; private set; }

    private SocketIO _socket;
    private TicketPck _ticketPck = null;
    private GameDataPck _gameDataPck = null;

    public void SendPlayerData(PlayerData playerData)
    {
        Debug.Assert(IsConnected);
        PlayerDataPck playerDataPck = new PlayerDataPck(playerData.id);
        Task.Run(() => _socket.EmitAsync(PacketNames.PlayerData, playerDataPck));
    }

    public void RequestMatch()
    {
        Debug.Assert(IsConnected);
        Task.Run(() => _socket.EmitAsync(PacketNames.RequestMatch, new RequestMatchPck()));
    }

    public void PeerSend(GameDataPck gameDataPck)
    {
        Task.Run(() => _socket.EmitAsync(PacketNames.GameData, gameDataPck));
    }

    public void CancelMatch()
    {
        Debug.Assert(IsConnected);
        Task.Run(() => _socket.EmitAsync(PacketNames.CancelMatch, ""));
    }

    private void Awake()
    {
        // singleton
        if (Singleton == null) Singleton = this;
        // create socket
        (string host, string port) = GetHostNPort();
        _socket = new SocketIO($"http://{host}:{port}");

        // connect to server
        Task.Run(() => _socket.ConnectAsync());
    }

    private void Start()
    {
        StartCoroutine(MainThreadBusyWaitingConnect());
        StartCoroutine(MainThreadBusyWaitingTicketPck());
        StartCoroutine(MainThreadBusyWaitingGameDataPck());
    }

    private IEnumerator MainThreadBusyWaitingConnect()
    {
        // busy wait until connected to server
        while (!IsConnected)
        {
            yield return new WaitForSeconds(0.2f);
        }

        // emit connect event
        print("connect to server");
        _socket.On(PacketNames.Ticket, (res) =>
        {
            TicketPck ticketPck = res.GetValue<TicketPck>();
            _ticketPck = ticketPck;
        });
        _socket.On(PacketNames.GameData, (res) =>
        {
            GameDataPck pck = res.GetValue<GameDataPck>();
            _gameDataPck = pck;

        });
        ConnectedEvent?.Invoke();
    }

    private IEnumerator MainThreadBusyWaitingTicketPck()
    {
        while (true)
        {
            while (_ticketPck == null)
            {
                yield return new WaitForSeconds(0.2f);
            }

            print("get ticket");
            //emit event
            GetTicketPckEvent?.Invoke(_ticketPck);
            // set null again
            _ticketPck = null;
        }
    }

    private IEnumerator MainThreadBusyWaitingGameDataPck()
    {
        while (true)
        {
            while (_gameDataPck == null)
            {
                yield return new WaitForSeconds(0.2f);
            }
            print($"get peer data: {_gameDataPck.DatatypeName}");
            PeerRecvEvent?.Invoke(_gameDataPck);
            _gameDataPck = null;
        }
    }

    
    private void OnDestroy()
    {
        if (_socket != null)
        {
            Task.Run(()=>_socket.DisconnectAsync());
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