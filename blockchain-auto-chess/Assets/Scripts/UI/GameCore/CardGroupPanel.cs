using System;
using System.Collections.Generic;
using Army;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameCore
{
    public class CardGroupPanel : MonoBehaviour
    {
        public event Action<int> CardTagEvent;
        public int SelectedIndex { get; private set; } = -1;
        public readonly Dictionary<int, CardFrame> CardFrames = new Dictionary<int, CardFrame>();
        private readonly Transform[] _slotsPos = new Transform[9];

        private void Awake()
        {
            int i = 0;
            foreach (Transform t in transform)
            {
                _slotsPos[i] = t;
                i++;
            }

            foreach (Transform slot in _slotsPos)
            {
                if (slot != null)
                {
                    slot.gameObject.SetActive(false);
                }
            }
        }

        public void ShowCards(Dictionary<int, SoldierFactory.SoldierType> hand)
        {
            foreach (var card in CardFrames.Values)
            {
                Destroy(card.gameObject);
            }

            CardFrames.Clear();

            foreach (var item in hand)
            {
                int index = item.Key;
                SoldierFactory.SoldierType soldierType = item.Value;
                GameObject card = Instantiate(soldierType._cardFrame, _slotsPos[index].position,
                    soldierType._cardFrame.transform.rotation, transform);
                CardFrames[index] = card.GetComponent<CardFrame>();
                card.GetComponent<Button>().onClick.AddListener(() => CardTagEvent?.Invoke(index));
            }
        }

        public void SelectCard(int index)
        {
            DeSelectAll();
            CardFrames[index].FocusCard(true);
            SelectedIndex = index;
        }

        public void DeSelectAll()
        {
            foreach (CardFrame cardFrame in CardFrames.Values)
            {
                cardFrame.FocusCard(false);
            }

            SelectedIndex = -1;
        }
    }
}