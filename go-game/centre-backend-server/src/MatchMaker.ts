import PacketType, { RequestMatchPck } from "./PacketType";
import { Player, PlayerManager, PlayerState } from "./PlayerManager"
import { ArrayremoveItem } from "./ulti";
import Relayer from "./Relayer";

export default class MatchMaker {
    private playerManager: PlayerManager | undefined;
    private relayer!: Relayer;
    private matchWaitingList: string[] = [];

    public Init(playerManager: PlayerManager, relayer: Relayer) {
        this.playerManager = playerManager;
        // Add player to waiting list when request
        this.playerManager.OnPlayerRequestMatch((player: Player, requestMatchPck: RequestMatchPck) => {
            playerManager.ChangePlayerState(player.Id, PlayerState.WaitingMatch);
            this.matchWaitingList.push(player.Id);
            // do match making when player are added
            this.DoMatchMaking();
        });
        // remove plyer from waiting list when cancel or player is removed
        this.playerManager.OnRemovePlayer((player: Player) => {
            ArrayremoveItem<string>(this.matchWaitingList, player.Id);
        });
        playerManager.OnPlayerCancelMatch((player: Player) => {
            ArrayremoveItem<string>(this.matchWaitingList, player.Id);
        })
    }

    private DoMatchMaking() {
        // ensure can do match making
        if (!this.playerManager || this.matchWaitingList.length < 2) return;
        // match players
        const [p1, p2] = this.FIFOMatching();
        // set up p2p connection
        this.relayer.P2PRelay(p1.Socket, p2.Socket);
        // give ticket to player
        this.playerManager.SendTicketToPlayer(p1, {
            p2pConnectMethod: P2PConnectMethod.socketIORelay,
            MethodSpecificData: {
                packetType: PacketType.GameData
            }
        })
    }

    private FIFOMatching(): [Player, Player] {
        let pp = this.matchWaitingList.slice(0, 2).map(x => this.playerManager!.players.get(x));
        let _pp: [Player, Player] = [pp[0]!, pp[1]!]
        this.matchWaitingList.splice(0, 2);
        return _pp;
    }
}

export interface Ticket {
    p2pConnectMethod: string;
    MethodSpecificData: any;
}

interface SocketIORelayData {
    channelName: string;
}

class P2PConnectMethod {
    static socketIORelay = "socket.io-relay";
}