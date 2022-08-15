using UnityEngine;

namespace Army.AI
{
    public enum ActionTypes
    {
        DoNothing,
        Attack,
        Move
    }
    
    public class Payload
    {
    }

    public class MoveActionPayload : Payload
    {
        public Vector2Int DVec { get; private set; }

        public MoveActionPayload(Vector2Int dVec)
        {
            DVec = dVec;
        }
    }

    public class AttackActionPayload : Payload
    {
        public Vector2Int AttackPos { get; private set; }

        public AttackActionPayload(Vector2Int attackPos)
        {
            AttackPos = attackPos;
        }
    }
}