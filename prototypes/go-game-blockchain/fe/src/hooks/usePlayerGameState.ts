import Moralis from "moralis/types";
import { useEffect, useState } from "react";
import { useMoralis, useMoralisSubscription } from "react-moralis";
import useGoGameContract, {
  FindMatch,
  GameState,
  PlayerState,
} from "./useGoGameContract";
import { BigNumber } from "@ethersproject/bignumber";

const usePlayerGameState = () => {
  const [matchId, setMatchId] = useState<string>("0");
  const [stoneType, setStoneType] = useState<number>(0);
  const [boardState, setBoradState] = useState<number[]>([]);
  const [whosTurn, setWhosTurn] = useState(0);
  const [isOver, setisOver] = useState(false);
  const { account } = useMoralis();
  const { runFunc: getPlayerState } = useGoGameContract({
    functionName: "MyPlayerState",
  });
  const { runFunc: getBoardState } = useGoGameContract({
    functionName: "BoardState",
    params: { matchId },
  });
  const { runFunc: getWhosTurn } = useGoGameContract({
    functionName: "WhosTurn",
    params: { matchId },
  });
  const { runFunc: getIsOver } = useGoGameContract({
    functionName: "IsOver",
    params: { matchId },
  });

  const fetchPlayerGameState = async () => {
    if (!account) {
      console.log("get web3 not ready");
      return;
    }
    // get player state
    const playerState = (await getPlayerState()) as PlayerState;
    console.log("get player state");
    console.log(playerState);
    console.log(`match id: ${matchId}`);
    // check if we need to update match id again
    if (playerState.matchId.gt(BigNumber.from(matchId))) {
      setMatchId(playerState.matchId.toString());
    } else {
      // update player state (stone type)
      setStoneType(playerState.stoneType);
      // get&set game state
      const boardState = (await getBoardState()) as number[];
      const whosTurn = (await getWhosTurn()) as number;
      const isOver = (await getIsOver()) as boolean;
      setBoradState(boardState);
      setWhosTurn(whosTurn);
      setisOver(isOver);
      console.log("get game state");
      console.log(boardState, whosTurn, isOver);
    }
  };


  useEffect(() => {
    fetchPlayerGameState();
  }, [matchId, account]);

  const onGetFindMatchEvent = async (
    data: Moralis.Object<Moralis.Attributes>
  ) => {
    // return if no web3 account
    if (!account) return;

    const findMatchattrs = data.attributes as FindMatch;
    // is this find match event include this player?
    if (
      findMatchattrs.player1 === account ||
      findMatchattrs.player2 === account
    ) {
      setMatchId(findMatchattrs.matchId.toString());
    }
  };

  useMoralisSubscription("FindMatch", (q) => q, [], {
    onCreate: onGetFindMatchEvent,
    onUpdate: onGetFindMatchEvent,
  });

  const OnGameStateChangeEvent = async (
    data: Moralis.Object<Moralis.Attributes>
  ) => {
    console.log("get game state change event")
    console.log(data)
    // return if no web3 account
    if (!account) return;

    let _matchId = data.attributes.matchId as BigNumber;
    if(_matchId.toString() === matchId) await fetchPlayerGameState();
  };

  useMoralisSubscription("GameStateChange", (q) => q, [], {
    onCreate: OnGameStateChangeEvent,
    onUpdate: OnGameStateChangeEvent,
  });

  const o: { playerState: PlayerState; gameState: GameState } = {
    playerState: {
      matchId: BigNumber.from(matchId),
      stoneType,
      inGame: true,
    },
    gameState: {
      boardState,
      turn: whosTurn,
      isOver,
    },
  };
  return o;
};

export default usePlayerGameState;
