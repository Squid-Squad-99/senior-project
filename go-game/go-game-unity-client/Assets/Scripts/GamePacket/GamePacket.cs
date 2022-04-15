using System;
using System.Linq;
using System.Text.Json;

namespace GamePacket;

public abstract class GamePck
{
    public string PckName { get; protected set; }

    protected GamePck(string pckName)
    {
        PckName = pckName;
    }
}

public class HandShake : GamePck
{
    
    public string SenderId { get; private set; }

    public HandShake(string senderId = "NoneId") : base("HandShake")
    {
        SenderId = senderId;
    }
}

public static class GamePackets
{
    private static Type[] GamePckTypes = null;
    
    public static void SetGamePckTypes()
    {
        var listOfGamePcks = (
            from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
            from assemblyType in domainAssembly.GetTypes()
            where assemblyType.IsSubclassOf(typeof(GamePck))
            select assemblyType
        ).ToArray();

        GamePckTypes = listOfGamePcks;
    }
}