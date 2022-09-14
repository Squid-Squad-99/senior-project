using System.Collections;
using Ultility;
using UnityEngine;

public class AutoPlayer : Singleton<AutoPlayer>
{
        private Player _player;

        public void StartAutoPlay(Player player)
        {
                _player = player;
                StartCoroutine(AutoPlay());
        }

        private IEnumerator AutoPlay()
        {
                while (true)
                {
                        yield return new WaitForSeconds(1f);
                        yield return new WaitUntil(() => _player.IsMyTurnToUsedCard);
                        // decide place index
                        Vector2Int soldierIndex = DecidePlaceIndex();
                        int cardIndex = DecideCardIndex();
                        _player.UseCard(cardIndex, soldierIndex);
                }
        }

        private Vector2Int DecidePlaceIndex()
        {
                for (int i = 0; i < 100; i++)
                {
                        int x = Random.Range(0, GameTiles.Instance.TileSize);
                        int y = Random.Range(0, GameTiles.Instance.TileSize);
                        Vector2Int index = new Vector2Int(x, y);
                        if (!GameTiles.Instance.IsIndexOccupied(index))
                        {
                                return index;
                        }
                        
                }
                return Vector2Int.zero;
        }

        private int DecideCardIndex()
        {
                foreach (int i in _player.CardInHand.Keys)
                {
                        return i;
                }

                return -1;
        }
}