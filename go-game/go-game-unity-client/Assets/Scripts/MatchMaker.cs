using System;
using System.Collections;
using UnityEngine;

public interface IMatchMaker
{
    bool HaveRegister { get; }
    event Action<Ticket> GetTicketEvent;
    void RegisterLocalPlayer(PlayerData localPlayerData);
    void RequestMatch();
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

    public void RegisterLocalPlayer(PlayerData localPlayerData)
    {
        Debug.Assert(MatchNRelaySocket.Singleton != null);
        if (MatchNRelaySocket.Singleton.IsConnected)
        {
            MatchNRelaySocket.Singleton.SendPlayerData(localPlayerData);
            HaveRegister = true;
        }
        else
        {
            MatchNRelaySocket.Singleton.ConnectedEvent += () =>
            {
                MatchNRelaySocket.Singleton.SendPlayerData(localPlayerData);
                HaveRegister = true;
            };
        }
    }
    
    public void RequestMatch()
    {
        
        Debug.Assert(HaveRegister);
        MatchNRelaySocket.Singleton.RequestMatch();
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
    public string P2PConnectMethod { get; private set; }

    public Ticket(string p2PConnectMethod)
    {
        P2PConnectMethod = p2PConnectMethod;
    }
}
