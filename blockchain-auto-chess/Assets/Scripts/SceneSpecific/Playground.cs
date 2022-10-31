using System;
using System.Collections;
using Army;
using UnityEngine;

namespace SceneSpecific
{
    public class Playground: MonoBehaviour
    {
        private IEnumerator Start()
        {
            var soilder = SoldierFactory.Instance.CreateSoldier(SoldierFactory.SoldierNameEnum.Base, new Vector2Int(2, 2),
                TeamColorTypes.Red, Vector2Int.up);
            yield return null;
            while (true)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    soilder.Move(Vector2Int.up);
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    soilder.Move(Vector2Int.down);
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    soilder.Move(Vector2Int.left);
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    soilder.Move(Vector2Int.right);
                }
                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    soilder.Attack(soilder.IndexPos + Vector2Int.up);
                }

                yield return null;
            }
        }
    }
}