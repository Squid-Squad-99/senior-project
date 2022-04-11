using System;
using System.Threading.Tasks;
using UnityEngine;

public interface IMatchMaker
{
    bool HaveRegister { get; }
    event Action<Ticket> GetTicketEvent;
    Task RegisterLocalPlayer(PlayerData localPlayerData);
    Task RequestMatch();
}

public class MatchMaker : MonoBehaviour, IMatchMaker
{
    // TODO: need to make sure socket is connected
    public bool HaveRegister { get; private set; }
    public event Action<Ticket> GetTicketEvent;

    private void OnGetTicketPck(TicketPck ticketPck)
    {
        GetTicketEvent?.Invoke(new Ticket(ticketPck.p2pConnectMethod));
    }

    public async Task RegisterLocalPlayer(PlayerData localPlayerData)
    {
        Debug.Assert(MatchNRelaySocket.Singleton != null);
        if (MatchNRelaySocket.Singleton.IsConnected)
        {
            await MatchNRelaySocket.Singleton.SendPlayerData(localPlayerData);
            HaveRegister = true;
        }
        else
        {
            MatchNRelaySocket.Singleton.ConnectedEvent += async () =>
            {
                await MatchNRelaySocket.Singleton.SendPlayerData(localPlayerData);
                HaveRegister = true;
            };
        }
    }
    
    public async Task RequestMatch()
    {
        
        Debug.Assert(HaveRegister);
        await MatchNRelaySocket.Singleton.RequestMatch();
    }
    
    private void Start()
    {
        MatchNRelaySocket.Singleton.GetTicketPckEvent+= OnGetTicketPck;
    }

    private void OnDestroy()
    {
        MatchNRelaySocket.Singleton.GetTicketPckEvent -= OnGetTicketPck;
    }
}

public class Ticket
{
    public string p2pConnectMethod { get; private set; }

    public Ticket(string p2PConnectMethod)
    {
        p2pConnectMethod = p2PConnectMethod;
    }
}
