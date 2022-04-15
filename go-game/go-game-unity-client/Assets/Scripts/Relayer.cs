using System;
using UnityEngine;

public interface IRelayer
{
    event Action<Byte[]> RecvEvent;
    void Send(Byte[] payLoad);
}

[RequireComponent(typeof(IMatchNRelayClient))]
public class Relayer : MonoBehaviour, IRelayer
{
    public event Action<Byte[]> RecvEvent;
    private IMatchNRelayClient _matchNRelayClient;

    public void Send(Byte[] payload)
    {
        _matchNRelayClient.RelaySend(payload);
    }

    private void Awake()
    {
        _matchNRelayClient = GetComponent<IMatchNRelayClient>();
        _matchNRelayClient.RelayRecvEvent += OnRelayRecv;
    }

    private void OnRelayRecv(byte[] payload)
    {
        RecvEvent?.Invoke(payload);
    }

    private void OnDestroy()
    {
        _matchNRelayClient.RelayRecvEvent -= OnRelayRecv;
    }
}