import {
  Contract,
  ContractReceipt,
  ContractTransaction,
} from "@ethersproject/contracts";
import { assert, expect } from "chai";
import { BigNumber } from "ethers";
import { ethers, deployments, getNamedAccounts } from "hardhat";

const printBoard = (board: number[]) => {
  let s: string[] = [];
  for (let y = 0; y < 19; y++) {
    let line: string = "";
    for(let x = 0; x < 19; x++){
      line += `${board[x + y * 19]} `;
    }
    s.push(line);
  }
  s = s.reverse();
  console.log("board state:");
  console.log(s);
}

describe("GoGame Unit Testing", () => {
  let goGame: Contract;
  let p1: string, p2: string;
  let p1Contract: Contract, p2Contract: Contract;
  beforeEach(async () => {
    // deploy contract
    await deployments.fixture(["GoGame"]);

    // set up varaible
    goGame = await ethers.getContract("GoGame");
    const namedAccounts = await getNamedAccounts();
    p1 = namedAccounts.player1;
    p2 = namedAccounts.player2;
    p1Contract = await ethers.getContract("GoGame", p1);
    p2Contract = await ethers.getContract("GoGame", p2);
  });

  it("should match player", async () => {
    // player1 request match
    const tx1: ContractTransaction = await p1Contract.requestMatch();
    await tx1.wait();
    // player2 request match
    const tx2: ContractTransaction = await p2Contract.requestMatch();
    await tx2.wait();
    await expect(tx2)
      .to.emit(goGame, "FindMatch")
      .withArgs(BigNumber.from("1"), p1, p2);
  });

  describe("testing in game", () => {
    let whitePlayerContract: Contract, blackPlayerContract: Contract;
    let matchId: BigNumber;
    beforeEach(async () => {
      // player1 request match
      const tx1: ContractTransaction = await p1Contract.requestMatch();
      await tx1.wait();
      // player2 request match
      const tx2: ContractTransaction = await p2Contract.requestMatch();
      await tx2.wait();

      // get white/black player
      const p1PS = await p1Contract.MyPlayerState();
      whitePlayerContract = p1PS.stoneType == 2? p1Contract: p2Contract;
      blackPlayerContract = p1PS.stoneType == 2? p2Contract: p1Contract;
      // match id
      matchId = p1PS.matchId;
    });

    it("player have different stone type", async () => {
      const p1PS = await p1Contract.MyPlayerState();
      const p2PS = await p2Contract.MyPlayerState();
      assert(p1PS.stoneType != p2PS.stoneType);
    })

    it("turn should change after placing", async () => {
      await whitePlayerContract.PlaceStone(1, 1, false);
      expect(await goGame.WhosTurn(matchId)).to.equal(1);
    });

    it("place invalid index should revert",async () => {
      await whitePlayerContract.PlaceStone(1, 1, false);
      await expect(blackPlayerContract.PlaceStone(1, 1, false)).to.be.revertedWith("GoGame__InvalidPlacing");
      await expect(blackPlayerContract.PlaceStone(20, 1, false)).to.be.revertedWith("GoGame__InvalidPlacing");
      await expect(blackPlayerContract.PlaceStone(1, 20, false)).to.be.revertedWith("GoGame__InvalidPlacing");
    });

    it("place stone",async () => {
      await whitePlayerContract.PlaceStone(1, 1, false);
      await blackPlayerContract.PlaceStone(2, 2, false);
      await whitePlayerContract.PlaceStone(3, 3, false);
      await blackPlayerContract.PlaceStone(4, 4, false);
      const board: number[] = await goGame.BoardState(matchId);
      printBoard(board);
    })
  });
});

