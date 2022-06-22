import { ethers, deployments, getNamedAccounts, network } from "hardhat";

const main = async () => {
  const { deployer, player1, player2 } = await getNamedAccounts();
  const goGame = await ethers.getContract("GoGame", deployer);
  const p1Cont = await ethers.getContract("GoGame", player1);
  const p2Cont = await ethers.getContract("GoGame", player2);
  const p1ps =  await p1Cont.MyPlayerState();
  const p2ps = await p2Cont.MyPlayerState();
  console.log(p1ps);
  console.log(p2ps);
};

main();