using System.Collections;
using System.Collections.Generic;
using Army;
using UnityEngine;

public class Player : MonoBehaviour
{
        public TeamColorTypes ArmyTeamColor { get; private set; }
        public Dictionary<int,SoldierFactory.SoldierType> CardInHand { get; private set; } = new Dictionary<int,SoldierFactory.SoldierType>();

        public void Init(TeamColorTypes armyTeamColor)
        {
                ArmyTeamColor = armyTeamColor;
        }

        public void ClearHand()
        {
                CardInHand.Clear();
        }

        public void FillHand(List<SoldierFactory.SoldierType> cards)
        {
                ClearHand();
                for (int i = 0; i < cards.Count; i++)
                {
                       CardInHand.Add(i, cards[i]);
                }
        }

        public IEnumerator MyTurnToPlaceSoldier()
        {
                yield break;
        }
        
        public void UseCard(int cardIndex, Vector2Int soldierPos)
        {
                SoldierFactory.SoldierType soldierType = CardInHand[cardIndex];
                CardInHand.Remove(cardIndex);
                SoldierFactory.Instance.CreateSoldier(soldierType._name, soldierPos, ArmyTeamColor);
        }
}