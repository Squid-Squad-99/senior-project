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

        private void OnEnable()
        {
            Relay.RelayClient.Instance.OnRecvPayload.AddListener(ListenRemotePlayerMove);

            localPlayer.useCardUnityEvent.AddListener(OnLocalPlayerUseCard);
        }

        private void OnDisable()
        {
            localPlayer.useCardUnityEvent.RemoveListener(OnLocalPlayerUseCard);
        }

        private void ListenRemotePlayerMove(BasePayload payload)
        {
            if(payload.PayloadType != (int)BasePayload.Type.Msg) return;
            GameMsg msg = new GameMsg(payload.Body);
            switch (msg.Type)
            {
                case GameMsgType.UseCard:
                    remotePlayer.UseCard(msg.CardIndex, new Vector2Int(msg.X, msg.Y));
                    break;
            }
        }

        private void OnLocalPlayerUseCard(int cardIndex, int x, int y)
        {
            StartCoroutine(
                Relay.RelayClient.Instance.SendMsgAsync(new GameMsg(GameMsgType.UseCard, cardIndex, x, y).Encode()));
        }
    }
}