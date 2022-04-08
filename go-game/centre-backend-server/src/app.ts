import { Socket } from "socket.io";
import SocketIOServer from "./SocketIOServer";
import PacketType, { IPlayerDataPckCxt } from "./PacketType";

const socketIOServer = new SocketIOServer();
socketIOServer.StartServer();

// only the player is onlined
enum PlayerState{
    WaitingMatch,
    InGame,
    idle,
}
class Player {
  Id: string;
  IsMatch: boolean = false;
  Match: Match | undefined = undefined;
  Socket: Socket;

  constructor(id: string, socket: Socket) {
    this.Id = id;
    this.Socket = socket;
  }

  GetMatch(match: Match){
      this.IsMatch = true;
      this.Match = match;
  }
}

class Match {
  static Count = 0;
  ID: string;
  Player1: Player;
  Player2: Player;
  constructor(p1: Player, p2: Player) {
    this.Player1 = p1;
    this.Player2 = p2;
    this.ID = `${++Match.Count}`;
    
    this.Player1.GetMatch(this);
    this.Player2.GetMatch(this);
  }
}

const PlayerMap = new Map<string, Player>(); // index by socket id
const Matches: Match[] = [];

socketIOServer.On(PacketType.Connection, (socket) => {
  console.log(`connect cnt: ${socketIOServer.connectedSockets.length}`);

  // deleget packets
  socket.on(PacketType.PlayerData, (cxt) => handlePlayerDataPck(socket, cxt));
  socket.on(PacketType.Disconnect, (reason) =>
    handleDisconnect(socket, reason)
  );

  DoMatchMaking();
});

const DoMatchMaking = () => {
  // loop all player, if two are not match then match them
  const matchedPlayers: Player[] = [];
  for (const [socketID, player] of PlayerMap.entries()) {
    if (player.IsMatch === false) {
      matchedPlayers.push(player);
    }
    if (matchedPlayers.length === 2) break;
  }
  if (matchedPlayers.length < 2) {
    // not enough player to be match
  } else {
      // create new match
    const match = new Match(matchedPlayers[0], matchedPlayers[1]);
    Matches.push(match);
  }
};

const handlePlayerDataPck = (socket: Socket, cxt: IPlayerDataPckCxt): void => {
  // add player data to map
  PlayerMap.set(socket.id, new Player(cxt.id, socket));
};

const handleDisconnect = (socket: Socket, reason: string): void => {
  PlayerMap.delete(socket.id);
};
