import React, { useState, useEffect } from "react";
import {
  useMoralis,
  useWeb3Contract,
  Web3ExecuteFunctionParameters,
} from "react-moralis";
import networkMapping from "../constants/networkMapping.json";
import abi from "../constants/abi.json";
import { BigNumber } from "@ethersproject/bignumber";

export interface PlayerState {
  stoneType: number;
  matchId: BigNumber;
  inGame: boolean;
}

export interface GameState {
  boardState: number[];
  turn: number;
  isOver: boolean;
}

export interface FindMatch {
  matchId: string;
  player1: string;
  player2: string;
}

type Props = {
  functionName:
    | "MyPlayerState"
    | "WhosTurn"
    | "BoardState"
    | "requestMatch"
    | "IsOver"
    | "GetGameState";
  params?: any;
};

const useGoGameContract = ({ functionName, params }: Props) => {
  const { chainId, account, isWeb3Enabled } = useMoralis();
  const chainString = chainId ? parseInt(chainId).toString() : "31337";
  const goGameAddress = (networkMapping as any)[chainString].GoGame[0];
  useEffect(() => {
    if (!isWeb3Enabled) {
      console.log("not connect to web3");
    } else {
      console.log(`game address ${goGameAddress}, chain id ${chainString}`);
    }
  }, [isWeb3Enabled, chainId]);

  // get player state
  const { runContractFunction: runFunc } = useWeb3Contract({
    abi: abi,
    contractAddress: goGameAddress,
    functionName: functionName,
    params: params ? params : {},
  });

  const ready = isWeb3Enabled;
  return {
    ready,
    runFunc,
  };
};

export default useGoGameContract;
