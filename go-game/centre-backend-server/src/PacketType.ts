class PacketType{
    static PlayerData = "PlayerData";
    static Disconnect = "disconnect";
    static Connection = "connection";   
}

interface IPlayerDataPckCxt{
    id: string;
}

export default PacketType
export {IPlayerDataPckCxt}