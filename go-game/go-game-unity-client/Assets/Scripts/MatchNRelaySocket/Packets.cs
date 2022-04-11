
using System.Net.NetworkInformation;

public class PacketNames
{
    public static string PlayerData = "PlayerData";
    public static string RequestMatch = "requestMatch";
    public static string CancelMatch = "cancelMatch";
    public static string GameData = "gameData";
    public static string Ticket = "ticket";
}

public class PlayerDataPck
{
    public string id { get; private set; }

    public PlayerDataPck(string id)
    {
        this.id = id;
    }
}

public class RequestMatchPck
{
    
}

public class TicketPck
{
    public string p2pConnectMethod { get; private set; }

    public TicketPck(string p2PConnectMethod)
    {
        p2pConnectMethod = p2PConnectMethod;
    }
}

public class GameDataNames
{
    public static string HandShake = "handShake";
    public static string PlaceStone = "placeStone";
    public static string GameOver = "gameOver";

}

public class GameDataPck
{
    public string DatatypeName { get; set; }
    public string Message { get; set; }
    public StoneType Winner { get; set; }
    public int XIndex { get; set; }
    public int YIndex { get; set; }

    public GameDataPck(string datatypeName, string message="", StoneType winner=0, int xIndex=-1, int yIndex=-1)
    {
        DatatypeName = datatypeName;
        Message = message;
        Winner = winner;
        XIndex = xIndex;
        YIndex = yIndex;
    }
}