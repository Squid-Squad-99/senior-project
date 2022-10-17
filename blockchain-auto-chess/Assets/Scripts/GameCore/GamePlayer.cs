using System;
using System.Collections;
using System.Collections.Generic;
using Army;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GameCore
{
    public class GamePlayer : MonoBehaviour
    {
        public TeamColorTypes armyTeamColor;

        // Player State
        // Game State
        public int WinRoundCount { get; private set; } = 0;
        // Round state
        public bool IsMyTurnToUsedCard { get; private set; } = false;

        public Dictionary<int, SoldierFactory.SoldierType> CardInHand { get; private set; } =
            new Dictionary<int, SoldierFactory.SoldierType>();

        public bool HaveCardIndex(int index)
        {
            return CardInHand.ContainsKey(index);
        }

        public event Action HandChangeEvent;
        public UnityEvent<int,int,int> useCardUnityEvent;

        public void ClearHand()
        {
            CardInHand.Clear();
            HandChangeEvent?.Invoke();
        }

        public void FillHand(List<SoldierFactory.SoldierType> cards)
        {
            ClearHand();
            for (int i = 0; i < cards.Count; i++)
            {
                CardInHand.Add(i, cards[i]);
                HandChangeEvent?.Invoke();
            }
        }

        public IEnumerator MyTurnToUseCard()
        {
            IsMyTurnToUsedCard = true;
            // ####
            yield return StartCoroutine(Ultility.UltiFunc.WaitUntilEvent(useCardUnityEvent));
            // ####
            IsMyTurnToUsedCard = false;
        }

        public void UseCard(int cardIndex, Vector2Int soldierPos)
        {
            
            SoldierFactory.SoldierType soldierType = CardInHand[cardIndex];
            CardInHand.Remove(cardIndex);
            HandChangeEvent?.Invoke();
            SoldierFactory.Instance.CreateSoldier(soldierType._name, soldierPos, armyTeamColor);

            useCardUnityEvent?.Invoke(cardIndex, soldierPos.x, soldierPos.y);

        }
    }
}