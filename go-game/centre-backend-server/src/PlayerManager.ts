import { Socket } from "socket.io";
import SocketIOServer from "./SocketIOServer";
import PacketType, { PlayerDataPck, RequestMatchPck } from "./PacketType";
import { ArrayremoveItem } from "./ulti";
import EventEmitter from "events";

class PlayerEventNames {
  static AddPlayer = "addPlayer";
  static RemovePlayer = "removePlayer";
}
type playerEventHandler = (player: Player) => void;

export class PlayerManager {
  public players: Map<string, Player> = new Map<string, Player>();
  private socketToPlayerMap: Map<string, Player> = new Map<string, Player>();
  private socketioServer: SocketIOServer;
  private playerEvent: EventEmitter = new EventEmitter();

  constructor(socketioServer: SocketIOServer) {
    this.socketioServer = socketioServer;
    this.MentainPlayers();
  }

  public ChangePlayerState(playerId: string, playerState: PlayerState){
      const player = this.players.get(playerId);
      if(player){
          player.state = playerState;
      }
  }

  public OnPlayerRequestMatch(
    handler: (player: Player, requestMatchPck: RequestMatchPck, ) => void,
  ): void {
    this.socketioServer.OnPacket(
      PacketType.RequestMatch,
      (socket: Socket, requestMatchPck: RequestMatchPck, callback) => {
        const player = this.socketToPlayerMap.get(socket.id);
        if (player) handler(player, requestMatchPck);
        else throw new Error("socket id dont have coresponed player object");
      }
    );
  }

  public OnAddPlayer(handler: playerEventHandler) {
    this.playerEvent.on(PlayerEventNames.AddPlayer, handler);
  }

  public OnRemovePlayer(handler: playerEventHandler) {
    this.playerEvent.on(PlayerEventNames.RemovePlayer, handler);
  }

  // auto mentain players
  private MentainPlayers() {
    // add player when get player data and auto remove when disconnect
    // add to players when give playerdata
    this.socketioServer.OnPacket(
      PacketType.PlayerData,
      (socket: Socket, playerData: PlayerDataPck) => {
        // add player
        const player = new Player(playerData.id, socket);
        this.socketToPlayerMap.set(socket.id, player);
        this.players.set(player.Id, player);
        // emit event
        this.playerEvent.emit(PlayerEventNames.AddPlayer, player);
      }
    );
    // remove player when disconnect
    this.socketioServer.OnDisconnect((socket: Socket, reason: string) => {
      // emit event
      const player = this.socketToPlayerMap.get(socket.id);
      if(player){
          this.playerEvent.emit(PlayerEventNames.RemovePlayer, player);
          // delete player
          this.socketToPlayerMap.delete(socket.id);
          this.players.delete(player.Id);
      }
    });
  }
}

// only the player is onlined
export enum PlayerState {
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
