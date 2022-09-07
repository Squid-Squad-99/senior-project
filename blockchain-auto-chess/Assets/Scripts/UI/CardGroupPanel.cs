using System;
using System.Collections.Generic;
using Army;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CardGroupPanel : MonoBehaviour
    {
        public event Action<int> CardTagEvent;
        private readonly Transform[] _slotsPos = new Transform[9];
        private readonly Dictionary<int, GameObject> _gameObjects = new Dictionary<int, GameObject>();
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
                slot.gameObject.SetActive(false);
            }
        }

        public void ShowCards(Dictionary<int, SoldierFactory.SoldierType> hand)
        {
            foreach (var card in _gameObjects.Values)
            {
                Destroy(card);
            }
            _gameObjects.Clear();
            
            foreach (var item in hand)
            {
                int index = item.Key;
                SoldierFactory.SoldierType soldierType = item.Value;
                GameObject card = Instantiate(soldierType._cardFrame, _slotsPos[index].position,  soldierType._cardFrame.transform.rotation, transform);
                _gameObjects[index] = card;
                card.GetComponent<Button>().onClick.AddListener(()=>CardTagEvent?.Invoke(index));
            }
        }

        public void SelectCard(int index)
        {
            
        }
    }
}
