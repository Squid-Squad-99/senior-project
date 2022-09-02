using System;
using System.Collections.Generic;
using Army;
using UnityEngine;

namespace UI
{
    public class CardGroupPanel : MonoBehaviour
    {
        private Transform[] slots = new Transform[9];
        private void Awake()
        {
            int i = 0;
            foreach (Transform t in transform)
            {
                slots[i] = t;
                i++;
            }

            foreach (Transform slot in slots)
            {
                slot.gameObject.SetActive(false);
            }
        }

        public void ShowCards(Dictionary<int, SoldierFactory.SoldierType> hand)
        {
            foreach (var item in hand)
            {
                int index = item.Key;
                SoldierFactory.SoldierType soldierType = item.Value;
                
            }
        }
    }
}
