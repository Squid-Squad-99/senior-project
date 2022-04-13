using System;
using UnityEngine;

public class Relayer : MonoBehaviour
{
    public event Action<GameDataPck> RecvEvent;

    public void Send(GameDataPck gameDataPck)
    {
        _matchNRelaySocket.PeerSend(gameDataPck);
    }

    
    private MatchNRelaySocket _matchNRelaySocket;
    private void Start()
    {
        _matchNRelaySocket = MatchNRelaySocket.Singleton;
        Debug.Assert(_matchNRelaySocket != null);

        _matchNRelaySocket.PeerRecvEvent += (gameDataPck) => RecvEvent?.Invoke(gameDataPck);
    }
}