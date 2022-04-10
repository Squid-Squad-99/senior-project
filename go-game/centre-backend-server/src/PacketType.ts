class PacketType {
    static Disconnect = "disconnect";
    static Connection = "connection";
    static PlayerData = "PlayerData";
    static RequestMatch = "requestMatch";
    static CancelMatch = "cancelMatch";
    static GameData = "gameData";
    static Ticket = "ticket";
}

export class StatusCode {
    static ok = "OK";
    static error = "ERROR";
}

export interface ResponsePck {
    status: string;
}

export interface RepsonseCallback{
    (response: ResponsePck): void;
} 

export interface PlayerDataPck {
    id: string;
}

export interface RequestMatchPck {

}

export interface RespondMatchPck {
    status: string;
}

export interface TicketPck{
    p2pConnectMethod: string;
    MethodSpecificData: any;
}

export default PacketType