// cli: yarn hardhat run scripts/requestMatch.ts --network localhost
import { BigNumber } from "ethers";
import { ethers, getNamedAccounts } from "hardhat";

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

const printBoard = (board: number[]) => {
  let s: string[] = [];
  for (let y = 0; y < 19; y++) {
    let line: string = "";
    for (let x = 0; x < 19; x++) {
      line += `${board[x + y * 19]} `;
    }
    s.push(line);
  }
  s = s.reverse();
  console.log("board state:");
  console.log(s);
};

async function PlayerGameState() {
  const { player1, player2 } = await getNamedAccounts();
  let goGame = await ethers.getContract("GoGame", player1);
  let tx = (await goGame.MyPlayerState()) as PlayerState;
  console.log(`player 1 match id: ${tx.matchId}`);
  console.log(`stone type: ${tx.stoneType}`);

  let goGame2 = await ethers.getContract("GoGame", player2);
  let tx2 = (await goGame2.MyPlayerState()) as PlayerState;
  console.log(`player 2 match id: ${tx2.matchId}`);
  console.log(`stone type: ${tx2.stoneType}`);

//   let tx3 = await goGame.BoardState(tx.matchId) as GameState;
  console.log("=====Game State=====")
  console.log(`is over: ${(await goGame.IsOver(tx.matchId))}`);
  console.log(`turn: ${(await goGame.WhosTurn(tx.matchId))}`);
  printBoard((await goGame.BoardState(tx.matchId)));
  console.log("==========")
}

PlayerGameState()
  .then(() => process.exit(0))
  .catch((error) => {
    console.error(error);
    process.exit(1);
  });
