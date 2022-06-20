import {
  Contract,
  ContractReceipt,
  ContractTransaction,
} from "@ethersproject/contracts";
import { assert, expect } from "chai";
import { ethers, deployments, getNamedAccounts } from "hardhat";

describe("Greeter", function () {
  it("Should return the new greeting once it's changed", async function () {
    // const Greeter = await ethers.getContractFactory("Greeter");
    // const greeter = await Greeter.deploy("Hello, world!");
    // await greeter.deployed();

    await deployments.fixture(['Greeter']);
    
    const greeter = (await ethers.getContract("Greeter")).connect((await getNamedAccounts()).player1);

    expect(await greeter.greet()).to.equal("Hello, world!");

    const setGreetingTx = await greeter.setGreeting("Hola, mundo!");

    // wait until the transaction is mined
    await setGreetingTx.wait();

    expect(await greeter.greet()).to.equal("Hola, mundo!");
  });
});

describe("GoGame Unit Testing", () => {
  let goGame: Contract;
  let p1: string, p2: string;
  let p1Contract: Contract, p2Contract: Contract;
  beforeEach(async () => {
    await deployments.fixture(["GoGame"]);

    goGame = await ethers.getContract("GoGame");
    const namedAccounts = await getNamedAccounts();
    p1 = namedAccounts.player1;
    p2 = namedAccounts.player2;
    p1Contract = await ethers.getContract("GoGame", p1);
    p2Contract = await ethers.getContract("GoGame", p2);
  });

  it("should match player", async () => {

    // let tx: ContractTransaction = await goGame.requestMatch();
    // let receipt: ContractReceipt = await tx.wait();
    const tx1: ContractTransaction=await p1Contract.requestMatch();
    tx1.wait();
    const tx2: ContractTransaction = await p2Contract.requestMatch();
    await expect(tx2)
      .to.emit(goGame, "FindMatch")
      .withArgs(p1, p2);
  });

  it("Say hello", async () => {
    expect(await goGame.hello()).to.equal("Hello you");
  });
});
