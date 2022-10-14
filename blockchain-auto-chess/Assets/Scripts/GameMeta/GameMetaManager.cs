using System;
using System.Collections;
using UnityEngine;
using Relay;

namespace GameMeta
{
    public class GameMetaManager : MonoBehaviour
    {
        
        private IEnumerator Start()
        {
            yield return Relay.RelayClient.Instance.ConnectServerAsync();
            yield return Relay.RelayClient.Instance.JoinRoomAsync(1);
            yield return GameManager.Instance.StartGameAsync();
        }
    }
}