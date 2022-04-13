using System;
using System.Collections;
using UnityEngine;


/// <summary>
/// p2p
/// 1. decide stone type
/// 2. set up go contestants
/// </summary>
public class PreGoGameSetUp : MonoBehaviour
{

    // reference
    private MatchMaker _matchMaker;
    private Relayer _relayer;

    private void Awake()
    {
        _matchMaker = GetComponent<MatchMaker>();
        _relayer = GetComponent<Relayer>();
    }

    public IEnumerator DoCoroutine()
    {
        // get ticket
        Ticket ticket = null;
        void WaitTicket(Ticket t){
            ticket = t;
            _matchMaker.GetTicketEvent -= WaitTicket;
        }
        _matchMaker.GetTicketEvent += WaitTicket;
        yield return new WaitUntil(() => ticket != null);
        ticket = null;
        
        // handshake with peer
        bool haveHandShake = false;
        _relayer.Send(new GameDataPck(GameDataNames.HandShake));
        void OnRecv(GameDataPck pck)
        {
            if (pck.DatatypeName != GameDataNames.HandShake) throw new ArgumentException("expect handShake");
            haveHandShake = true;
            _relayer.RecvEvent -= OnRecv;
        }
        _relayer.RecvEvent += OnRecv;
        yield return new WaitUntil(() => haveHandShake);
        
        

    }
}