using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Army
{
    public class Soldier : MonoBehaviour
    {
        public Vector2Int IndexPos { get; private set; }
        public Vector2Int FaceDir { get; private set; }
        protected virtual Vector2Int[] AttackPoints => new[] {Vector2Int.up};
        public int TeamID { get; private set; }


        // reference
        private GameTiles _gameTiles;

        private void Awake()
        {
            _gameTiles = GameTiles.Instance;
        }

        private void SetIndexPos(Vector2Int pos)
        {
            // clamp in valid index
            pos.x = Mathf.Clamp(pos.x, 0, 7);
            pos.y = Mathf.Clamp(pos.y, 0, 7);
            // position
            IndexPos = pos;
            transform.position = _gameTiles.data[pos.x, pos.y].position;
        }

        private void SetFaceDir(Vector2Int faceDir)
        {
            // check is valid direction
            if (Math.Abs(faceDir.magnitude - 1) > 0.1f) return;
            // face
            FaceDir = faceDir;
            float turnDegree = faceDir.x == 1 ? 0 : faceDir.x == -1 ? 180 : faceDir.y == 1 ? -90 : 90;
            transform.rotation = Quaternion.Euler(0, turnDegree, 0);
        }

        public void Init(Vector2Int position, Vector2Int faceDirection, int teamID = 0)
        {
            SetIndexPos(position);
            SetFaceDir(faceDirection);
            FaceDir = Vector2Int.up;
            TeamID = teamID;
        }

        public void Move(Vector2Int dVec)
        {
            SetIndexPos(new Vector2Int(IndexPos.x + dVec.x, IndexPos.y + dVec.y));
        }

        public void Turn(Vector2Int faceDirection)
        {
            SetFaceDir(faceDirection);
        }

        [SerializeField] private GameObject _debugSquare;

        public void Attack()
        {
            Vector2Int[] attackIndices = new Vector2Int[AttackPoints.Length];
            // rotate matrix 
            int[] rotateMatrix = new int[4];
            if (FaceDir == Vector2Int.up)
            {
                rotateMatrix = new[] {1, 0, 0, 1};
            }
            else if (FaceDir == Vector2Int.down)
            {
                rotateMatrix = new[] {-1, 0, 0, -1};
            }
            else if (FaceDir == Vector2Int.right)
            {
                rotateMatrix = new[] {0, 1, -1, 0};
            }
            else if (FaceDir == Vector2Int.left)
            {
                rotateMatrix = new[] {0, -1, 1, 0};
            }
            else
            {
                throw new ArgumentException($"cant handle face direction {FaceDir}");
            }

            for (int i = 0; i < AttackPoints.Length; i++)
            {
                Vector2Int point = AttackPoints[i];
                Vector2Int rotatedPoint = new Vector2Int(
                    point.x * rotateMatrix[0] + point.y * rotateMatrix[1],
                    point.x * rotateMatrix[2] + point.y * rotateMatrix[3]);
                attackIndices[i] = rotatedPoint + IndexPos;
            }

            foreach (Vector2Int index in attackIndices)
            {
                StartCoroutine(Create1Sec(index, _debugSquare));
            }
        }

        private IEnumerator Create1Sec(Vector2Int index, GameObject prefab)
        {
            GameObject obj = Instantiate(prefab, _gameTiles.data[index.x, index.y].position, prefab.transform.rotation);
            yield return new WaitForSeconds(1);
            Destroy(obj);
        }
    }
}