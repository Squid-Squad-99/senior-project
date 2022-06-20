"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const chai_1 = require("chai");
const hardhat_1 = require("hardhat");
// describe("Greeter", function () {
//   it("Should return the new greeting once it's changed", async function () {
//     // const Greeter = await ethers.getContractFactory("Greeter");
//     // const greeter = await Greeter.deploy("Hello, world!");
//     // await greeter.deployed();
//     await deployments.fixture(['Greeter']);
//     const greeter = await ethers.getContract("Greeter")
//     expect(await greeter.greet()).to.equal("Hello, world!");
//     const setGreetingTx = await greeter.setGreeting("Hola, mundo!");
//     // wait until the transaction is mined
//     await setGreetingTx.wait();
//     expect(await greeter.greet()).to.equal("Hola, mundo!");
//   });
// });
describe("GoGame Unit Testing", () => {
    let goGame;
    let player1, player2;
    beforeEach(async () => {
        await hardhat_1.deployments.fixture(['GoGame']);
        goGame = await hardhat_1.ethers.getContract("GoGame");
        const namedAccounts = await (0, hardhat_1.getNamedAccounts)();
        player1 = namedAccounts.player1;
        player2 = namedAccounts.player2;
    });
    it("should match player", async () => {
        // player 1 request match
        goGame.connect(player1);
        await goGame.requestMatch();
        // player 2 request match
        goGame.connect(player2);
        let tx = await goGame.requestMatch();
        let receipt = await tx.wait();
        2 + 2;
    });
    it("Say hello", async () => {
        (0, chai_1.expect)(await goGame.hello()).to.equal("Hello you");
    });
});
