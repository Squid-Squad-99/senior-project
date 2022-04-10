class PacketType{
    static Disconnect = "disconnect";
    static Connection = "connection";   
    static PlayerData = "PlayerData";
    static RequestMatch = "requestMatch";
}

export interface PlayerDataPck{
    id: string;
}

export interface RequestMatchPck{

}

export interface RespondMatchPck{
    status: string;
}



export default PacketType