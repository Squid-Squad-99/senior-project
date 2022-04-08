import { Socket } from "socket.io";
import SocketIOServer from "./SocketIOServer";

export class PlayerManager{
    players: Player[] = []

    constructor(socketioServer: SocketIOServer){
        // auto remove and add player when connect to server
    }
}

// only the player is onlined
export enum PlayerState{
    WaitingMatch,
    InGame,
    idle,
}
export class Player {
  Id: string;
  state: PlayerState = PlayerState.idle;
  Socket: Socket;

  constructor(id: string, socket: Socket) {
    this.Id = id;
    this.Socket = socket;
  }

}