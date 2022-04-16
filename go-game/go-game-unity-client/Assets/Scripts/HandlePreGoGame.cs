using System;
using System.Collections;
using System.Data;
using GoGameProtocol;
using UnityEngine;


/// <summary>
/// p2p
/// 1. decide stone type
/// 2. set up go contestants
/// </summary>
public class HandlePreGoGame : MonoBehaviour
{
    // reference
    private IMatchMaker _matchMaker;
    private IPlayer _localPlayer;
    private GamePacketSocket _gamePacketSocket;

    private void Awake()
    {
        _matchMaker = GetComponent<IMatchMaker>();
        _localPlayer = GetComponent<IPlayer>();
        _gamePacketSocket = GetComponent<GamePacketSocket>();
    }

    public IEnumerator DoCoroutine()
    {
        ListenNWaitTicket.Listen(_matchMaker);
        ListenNWaitHandShake.Listen(_gamePacketSocket);
        
        // Request match
        print("request match");
        _matchMaker.RequestMatch();
        // get ticket
        print("waiting for ticket...");
        yield return ListenNWaitTicket.Wait();
        print("get ticket");

        // handshake with peer
        print("send and wait for handshake...");
        _gamePacketSocket.Send(new HandShakePck(){SenderId = _localPlayer.Data.id});
        yield return ListenNWaitHandShake.Wait();
        print("get handshake");
    }

    private static class ListenNWaitTicket
    {
        private static bool _haveGetTicket = false;

        public static void Listen(IMatchMaker matchMaker)
        {
            void WaitTicket(Ticket t)
            {
                _haveGetTicket = true;
                matchMaker.GetTicketEvent -= WaitTicket;
            }

            matchMaker.GetTicketEvent += WaitTicket;
        }

        public static IEnumerator Wait()
        {
            yield return new WaitUntil(() => _haveGetTicket);
        }
    }

     private static class ListenNWaitHandShake
    {
        private static bool _haveGetHandShake = false;

        public static void Listen(GamePacketSocket socket)
        {
            void OnGetHandShake(HandShakePck pck)
            {
                _haveGetHandShake = true;
                socket.GetHandShakePckEvent -= OnGetHandShake;
            }

            socket.GetHandShakePckEvent += OnGetHandShake;
        }

        public  static IEnumerator Wait()
        {
            yield return new WaitUntil(() => _haveGetHandShake);
        }
    }
}