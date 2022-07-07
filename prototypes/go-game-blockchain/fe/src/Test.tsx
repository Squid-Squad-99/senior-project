import { useAppContext } from "context/AppContextProvider";
import useGoGameContract, {
  FindMatch,
  PlayerState,
} from "hooks/useGoGameContract";
import React, { useEffect, useState } from "react";
import { ConnectButton } from "web3uikit";

import RequestMatchButton from "./components/RequestMatchButton";
import GameView from "components/GameView";
import usePlayerGameState from "hooks/usePlayerGameState";
import { useMoralis } from "react-moralis";

const Test = () => {
  const { metaGameState, playerState, gameState, setMetaGameState } =
    useAppContext();
  const { account } = useMoralis();
  useEffect(() => {
    // check is in game
    if (playerState.matchId.toString() !== "0" && gameState.isOver === false) {
      setMetaGameState("InGame");
    }
  }, [gameState, playerState]);

  useEffect(() => {
    // set not account
    if (!account) {
      setMetaGameState("NoAccount");
    } else if (metaGameState !== "InGame") {
      setMetaGameState("Idle");
    }
  }, [account]);

  return (
    <>
      {/* Header setion */}
      <div className="py-4 px-4 flex flex-row justify-between items-center bg-sky-300 border-b-4 border-slate-600 shadow">
        <div className="self-center font-bold text-xl">Blockchain Game</div>
        <div className="flex flex-row space-x-2">
          {metaGameState === "Idle" ? (
            <RequestMatchButton />
          ) : metaGameState === "SendingMatchRequest" ? (
            <div className="font-bold py-1.5 px-3 rounded-2xl ml-auto">
              sending match request...
            </div>
          ) : metaGameState === "FindingMatch" ? (
            <div className="font-bold py-1.5 px-3 rounded-2xl ml-auto">
              Finding Match...
            </div>
          ) : metaGameState === "InGame" ? (
            <div className="font-bold py-1.5 px-3 rounded-2xl ml-auto">
              Match Id: {playerState.matchId.toString()}
            </div>
          ) : (
            <div>meta game state wrong</div>
          )}
          <ConnectButton moralisAuth={false} />
        </div>
      </div>
      {/* Game View */}
      {metaGameState === "InGame" ? (
        <div>
          <GameView />
        </div>
      ) : (
        <div>not in game...</div>
      )}
    </>
  );
};

export default Test;
