import { RequestMatchPck } from "./PacketType";
import { Player, PlayerManager, PlayerState } from "./PlayerManager"
import { ArrayremoveItem } from "./ulti";

export default class MatchMaking{
    private playerManager: PlayerManager;
    private matchWaitingList: string[] = [];

    constructor(playerManager: PlayerManager){
        this.playerManager = playerManager;
        this.playerManager.OnPlayerRequestMatch((player: Player, requestMatchPck: RequestMatchPck)=>{
            playerManager.ChangePlayerState(player.Id, PlayerState.WaitingMatch);
            this.matchWaitingList.push(player.Id);
            this.DoMatchMaking();
        });
        this.playerManager.OnRemovePlayer((player: Player)=>{
            // if player is in waiting list, remove it
            if(this.matchWaitingList.includes(player.Id)){
                ArrayremoveItem<string>(this.matchWaitingList, player.Id);
            }
        });
    }
    DoMatchMaking() {
        throw new Error("Method not implemented.");
    }
}