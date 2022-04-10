import { Socket } from "socket.io";
import SocketIOServer from "./SocketIOServer";
import PacketType, { PlayerDataPck, RequestMatchPck } from "./PacketType";
import { Player, PlayerManager } from "./PlayerManager";

const socketIOServer = new SocketIOServer();
const playerManager = new PlayerManager(socketIOServer);

socketIOServer.StartServer();

// test();

const testConnection = () => {
  socketIOServer.OnConnect((socket) =>
    console.log(
      `new connection, connect cnt: ${socketIOServer.connectedSockets.length}`
    )
  );
  socketIOServer.OnDisconnect((socket: Socket, reason: string) =>
    console.log(`socket disconnect, socket id ${socket.id} reason: ${reason}`)
  );
};

const testPlayerData = () => {
  socketIOServer.OnPacket(
    PacketType.PlayerData,
    (socket: Socket, playerData: PlayerDataPck) => {
      console.log(
        `Get player data:\nplayer id: ${playerData.id}\nsocket id:${socket.id}`
      );
    }
  );
};

const testPlayerManager = () => {
  playerManager.OnAddPlayer((player: Player) =>
    console.log(`player added player id: ${player.Id}`)
  );
  playerManager.OnRemovePlayer((player: Player) =>
    console.log(`player removed player id ${player.Id}`)
  );
  playerManager.OnPlayerRequestMatch(
    (player: Player, requestMatchPck: RequestMatchPck) => {
      console.log(`player ${player.Id} request match`);
    }
  );
};

const test = () => {
  testConnection();
  testPlayerData();
  testPlayerManager();
};

