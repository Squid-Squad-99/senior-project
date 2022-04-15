using System;
using System.Collections;
using UnityEngine;


/// <summary>
/// p2p
/// 1. decide stone type
/// 2. set up go contestants
/// </summary>
public class PreGoGameNetworkCommunication : MonoBehaviour
{

    // reference
    private IMatchMaker _matchMaker;
    private IRelayer _relayer;
    private IPlayer _localPlayer;

    private void Awake()
    {
        _matchMaker = GetComponent<IMatchMaker>();
        _relayer = GetComponent<IRelayer>();
        _localPlayer = GetComponent<IPlayer>();
    }

    public IEnumerator DoCoroutine()
    {
        
        // Request match
        print("request match");
        _matchMaker.RequestMatch();
        // get ticket
        print("waiting for ticket...");
        yield return WaitForTicket();
        print("get ticket");
        
        // handshake with peer
        // _relayer.Send();
        
    }

    private IEnumerator WaitForTicket()
    {
        // get ticket
        bool haveGetTicket = false;
        void WaitTicket(Ticket t)
        {
            haveGetTicket = true;
            _matchMaker.GetTicketEvent -= WaitTicket;
        }
        _matchMaker.GetTicketEvent += WaitTicket;
        yield return new WaitUntil(() => haveGetTicket);
    }
}