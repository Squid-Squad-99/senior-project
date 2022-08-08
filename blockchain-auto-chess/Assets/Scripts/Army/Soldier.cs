using System;
using Unity.Mathematics;
using UnityEngine;

namespace Army
{
    public class Soldier : MonoBehaviour
    {
        public Vector2Int IndexPos { get; private set; }
        public Vector2Int FaceDir { get; private set; }
        public virtual Vector2Int[] AttackPoints => new[] {Vector2Int.up};
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
            if(Math.Abs(faceDir.magnitude - 1) > 0.1f) return;
            // face
            FaceDir = faceDir;
            float turnDegree = faceDir.x == 1 ? 0 : faceDir.x == -1 ? 180 : faceDir.y == 1 ? -90 : 90;
            transform.rotation = Quaternion.Euler(0,turnDegree,0);
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

        public void Attack()
        {
            
        }
    }
}