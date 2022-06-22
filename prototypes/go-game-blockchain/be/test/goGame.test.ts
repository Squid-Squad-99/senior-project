import {
  Contract,
  ContractReceipt,
  ContractTransaction,
} from "@ethersproject/contracts";
import { assert, expect } from "chai";
import { BigNumber } from "ethers";
import { ethers, deployments, getNamedAccounts, network } from "hardhat";

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

describe("GoGame Unit Testing", () => {
  let goGame: Contract;
  let p1: string, p2: string;
  let p1Contract: Contract, p2Contract: Contract;
  beforeEach(async () => {
    // deploy contract if in local otherwise we need to deploy manaully before running test
    if (network.name == "localhost" || network.name == "hardhat") {
      await deployments.fixture(["GoGame"]);
    }

    // set up varaible
    const namedAccounts = await getNamedAccounts();
    goGame = await ethers.getContract("GoGame", namedAccounts.deployer);
    p1 = namedAccounts.player1;
    p2 = namedAccounts.player2;
    p1Contract = await ethers.getContract("GoGame", p1);
    p2Contract = await ethers.getContract("GoGame", p2);
  });

  it("should match player", async () => {
    // player1 request match
    const tx1: ContractTransaction = await p1Contract.requestMatch();
    // player2 request match
    const tx2: ContractTransaction = await p2Contract.requestMatch();
    await tx1.wait();
    await tx2.wait();
    await expect(tx2)
      .to.emit(goGame, "FindMatch")
      .withArgs((await p1Contract.MyPlayerState()).matchId, p1, p2);
  });

  describe("testing in game (single game)", () => {
    let whitePlayerContract: Contract, blackPlayerContract: Contract;
    let matchId: BigNumber;
    beforeEach(async () => {
      // player1 request match
      const tx1: ContractTransaction = await p1Contract.requestMatch();
      // player2 request match
      const tx2: ContractTransaction = await p2Contract.requestMatch();
      await tx1.wait();
      await tx2.wait();

      // get white/black player
      const p1PS = await p1Contract.MyPlayerState();
      console.log(`Testing game with match id: ${p1PS.matchId}`);
      whitePlayerContract = p1PS.stoneType == 2 ? p1Contract : p2Contract;
      blackPlayerContract = p1PS.stoneType == 2 ? p2Contract : p1Contract;
      // match id
      matchId = p1PS.matchId;
    });

    it("player have different stone type", async () => {
      const p1PS = await p1Contract.MyPlayerState();
      const p2PS = await p2Contract.MyPlayerState();
      assert(p1PS.stoneType != p2PS.stoneType);
    });

    it("turn should change after placing", async () => {
      const tx: ContractTransaction = await whitePlayerContract.PlaceStone(1, 1, false);
      await tx.wait();
      expect(await goGame.WhosTurn(matchId)).to.equal(1);
    });

    it("should emit game state change event", async () => {
      await expect(whitePlayerContract.PlaceStone(1, 1, false))
        .to.emit(goGame, "GameStateChange")
        .withArgs(matchId);
    });

    it("place invalid index should revert", async () => {
      (await whitePlayerContract.PlaceStone(1, 1, false)).wait();
      await expect(
        blackPlayerContract.PlaceStone(1, 1, false)
      ).to.be.revertedWith("GoGame__InvalidPlacing");
      await expect(
        blackPlayerContract.PlaceStone(20, 1, false)
      ).to.be.revertedWith("GoGame__InvalidPlacing");
      await expect(
        blackPlayerContract.PlaceStone(1, 20, false)
      ).to.be.revertedWith("GoGame__InvalidPlacing");
    });

    describe("test all kind of win way", () => {
      it("line / white win", async () => {
        (await whitePlayerContract.PlaceStone(1, 1, false)).wait();
        (await blackPlayerContract.PlaceStone(10, 10, false)).wait();
        (await whitePlayerContract.PlaceStone(2, 2, false)).wait();
        (await blackPlayerContract.PlaceStone(11, 10, false)).wait();
        (await whitePlayerContract.PlaceStone(3, 3, false)).wait();
        (await blackPlayerContract.PlaceStone(13, 10, false)).wait();
        (await whitePlayerContract.PlaceStone(5, 5, false)).wait();
        (await blackPlayerContract.PlaceStone(12, 10, false)).wait();
        const tx = await whitePlayerContract.PlaceStone(4, 4, true);
        await expect(tx)
          .to.emit(goGame, "GameOver")
          .withArgs(matchId, await whitePlayerContract.signer.getAddress());
        const board: number[] = await goGame.BoardState(matchId);
        // printBoard(board);
      });

      it("line \\ white win", async () => {
        (await whitePlayerContract.PlaceStone(5, 5, false)).wait();
        (await blackPlayerContract.PlaceStone(10, 10, false)).wait();
        (await whitePlayerContract.PlaceStone(4, 6, false)).wait();
        (await blackPlayerContract.PlaceStone(12, 10, false)).wait();
        (await whitePlayerContract.PlaceStone(6, 4, false)).wait();
        (await blackPlayerContract.PlaceStone(13, 10, false)).wait();
        (await whitePlayerContract.PlaceStone(7, 3, false)).wait();
        (await blackPlayerContract.PlaceStone(11, 10, false)).wait();
        const tx = await whitePlayerContract.PlaceStone(3, 7, true);
        await expect(tx)
          .to.emit(goGame, "GameOver")
          .withArgs(matchId, await whitePlayerContract.signer.getAddress());
        const board: number[] = await goGame.BoardState(matchId);
        // printBoard(board);
      });
    });
  });
});
