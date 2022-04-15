using System;
using System.Collections;
using System.Threading.Tasks;
using dotenv.net;
using SocketIOClient;
using UnityEngine;


public interface IMatchNRelayClient
{
    event Action ConnectedEvent;
    event Action<TicketPck> GetTicketPckEvent;
    event Action<Byte[]> RelayRecvEvent;
    bool IsConnected { get; }
    void SendPlayerData(PlayerData playerData);
    void RequestMatch();
    void RelaySend(Byte[] payLoad);
}

/// <summary>
/// socket to match making server & relay server
/// try to connect to server when load
/// </summary>
public class MatchNRelayClient : MonoBehaviour, IMatchNRelayClient
{
    public event Action ConnectedEvent; // emit when socket is readied
    public event Action<TicketPck> GetTicketPckEvent;
    public event Action<Byte[]> RelayRecvEvent;
    public bool IsConnected => _socket.Connected;
    public static MatchNRelayClient Singleton { get; private set; }

    private SocketIO _socket;
    private TicketPck _ticketPck = null;
    private Byte[] _relayPayLoad = null;

    public void SendPlayerData(PlayerData playerData)
    {
        Debug.Assert(IsConnected);
        PlayerDataPck playerDataPck = new PlayerDataPck(playerData.id);
        Task.Run(() => _socket.EmitAsync(SocketIOEventNames.PlayerData, playerDataPck));
    }

    public void RequestMatch()
    {
        Debug.Assert(IsConnected);
        Task.Run(() => _socket.EmitAsync(SocketIOEventNames.RequestMatch, new RequestMatchPck()));
    }

    public void RelaySend(Byte[] payLoad)
    {
        Task.Run(() => _socket.EmitAsync(SocketIOEventNames.RelayData, payLoad));
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
        yield return new WaitUntil(() => IsConnected);

        // emit connect event
        print("connect to server");
        _socket.On(SocketIOEventNames.Ticket, (res) =>
        {
            TicketPck ticketPck = res.GetValue<TicketPck>();
            _ticketPck = ticketPck;
        });
        _socket.On(SocketIOEventNames.RelayData, (res) =>
        {
            Byte[] playLoad = res.GetValue<Byte[]>();
            _relayPayLoad = playLoad;
        });
        ConnectedEvent?.Invoke();
    }

    private IEnumerator MainThreadBusyWaitingTicketPck()
    {
        while (true)
        {
            yield return new WaitUntil(() => _ticketPck != null);

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
            yield return new WaitUntil(() => _relayPayLoad != null);

            print($"get peer data");
            RelayRecvEvent?.Invoke(_relayPayLoad);
            _relayPayLoad = null;
        }
    }

    private void OnDestroy()
    {
        if (_socket != null)
        {
            Task.Run(() => _socket.DisconnectAsync());
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