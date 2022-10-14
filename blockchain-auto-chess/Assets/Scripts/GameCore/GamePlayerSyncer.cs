using System;
using System.Collections;
using Relay.Payload;
using Ultility;
using UnityEngine;

namespace GameCore
{
    public class GamePlayerSyncer : MonoBehaviour
    {
        [SerializeField] private GamePlayer localPlayer;
        [SerializeField] private GamePlayer remotePlayer;

        private void Awake()
        {
            StartCoroutine(ListenRemotePlayerMove());
            
            localPlayer.useCardUnityEvent.AddListener(OnLocalPlayerUseCard);
        }

        private IEnumerator ListenRemotePlayerMove()
        {
            yield return Relay.RelayClient.Instance.WaitUntilRecvType(BasePayload.Type.Msg);
            GameMsg msg = new GameMsg(Relay.RelayClient.Instance.RecvPayload.Body);
            switch (msg.Type)
            {
                case GameMsgType.UseCard:
                    Debug.Log("remote use card");
                    remotePlayer.UseCard(msg.CardIndex, new Vector2Int( msg.X, msg.Y));
                    break;
            }
        }

        private void OnLocalPlayerUseCard(int cardIndex, int x, int y)
        {
            Debug.Log("local use card");
            StartCoroutine(Relay.RelayClient.Instance.SendMsgAsync(new GameMsg(GameMsgType.UseCard,cardIndex, x, y).Encode()));
        }
    }
}