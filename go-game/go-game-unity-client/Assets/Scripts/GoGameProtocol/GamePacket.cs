namespace GoGameProtocol
{
    public class GamePck
    {
        public string PckName { get; set; }
        
    }

    public class HandShakePck : GamePck
    {
        public string SenderId { get; set; }

        public HandShakePck()
        {
            PckName = GamePckNames.HandShake;
        }
    }

    public static class GamePckNames
    {
        public const string HandShake = "HandShake";
    }
}