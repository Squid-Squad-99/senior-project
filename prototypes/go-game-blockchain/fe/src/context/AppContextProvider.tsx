import useGoGameContract, { GameState, PlayerState } from "hooks/useGoGameContract";
import usePlayerGameState from "hooks/usePlayerGameState";
import React, { useState, createContext, useContext } from "react";
import { useMoralis } from "react-moralis";
import { BigNumber } from "@ethersproject/bignumber";

export interface IAppContext {
  metaGameState: MetaGameState;
  setMetaGameState: React.Dispatch<React.SetStateAction<MetaGameState>>;
  playerState: PlayerState,
  gameState: GameState,
}

type MetaGameState = "NoAccount" | "Idle" | "SendingMatchRequest" | "FindingMatch" | "InGame" |"GameOver";

export const AppContext = createContext<IAppContext>({
  metaGameState: "Idle",
  setMetaGameState: () => {},
  playerState: {matchId: BigNumber.from("0"), stoneType: 0, inGame: false},
  gameState: {boardState: [], turn: 0, isOver: true},
});

export const useAppContext = () => useContext(AppContext);

type Props = {
  children: React.ReactNode;
};

const AppStateProvider = (props: Props) => {
  const [metaGameState, setMetaGameState] = useState<MetaGameState>("Idle");
  const {playerState, gameState} = usePlayerGameState();
  const AppState: IAppContext = {
    metaGameState,
    setMetaGameState,
    playerState,
    gameState
  };
  return (
    <AppContext.Provider value={AppState}>{props.children}</AppContext.Provider>
  );
};

export default AppStateProvider;
