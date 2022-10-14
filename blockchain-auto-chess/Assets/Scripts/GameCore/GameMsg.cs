using System;

namespace GameCore
{
    public class GameMsg
    {
        public readonly GameMsgType Type;
        public readonly int CardIndex;
        public readonly int X;
        public readonly int Y;

        public GameMsg(GameMsgType type, int cardIndex, int x, int y)
        {
            Type = type;
            CardIndex = cardIndex;
            X = x;
            Y = y;
        }

        public GameMsg(byte[] encode)
        {
            Type = (GameMsgType)BitConverter.ToInt32(encode);
            CardIndex = BitConverter.ToInt32(encode, 4);
            X = BitConverter.ToInt32(encode, 8);
            Y = BitConverter.ToInt32(encode, 12);
        }

        public byte[] Encode()
        {
            byte[] encoded = new byte[16];
            Array.Copy(BitConverter.GetBytes((int)Type), 0, encoded, 0, 4);
            Array.Copy(BitConverter.GetBytes(CardIndex), 0, encoded, 4, 4);
            Array.Copy(BitConverter.GetBytes(X), 0, encoded, 8, 4);
            Array.Copy(BitConverter.GetBytes(Y), 0, encoded, 12, 4);
            return encoded;
        }
        
    }

    public enum GameMsgType
    {
        UseCard,
    }
}