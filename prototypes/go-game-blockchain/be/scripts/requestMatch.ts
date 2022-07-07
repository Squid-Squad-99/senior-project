// cli: yarn hardhat run scripts/requestMatch.ts --network localhost
import { ethers, getNamedAccounts } from "hardhat"

async function requestMatch() {
    const {player1, player2} = await getNamedAccounts();
    let goGame = await ethers.getContract("GoGame", player1)
    let tx = await goGame.requestMatch()
    await tx.wait(1)
    console.log("player 1 Match requested!")

    let goGame2 = await ethers.getContract("GoGame", player2)
    let tx2 = await goGame2.requestMatch()
    await tx2.wait(1)
    console.log("player 2 Match requested!")
}

requestMatch()
    .then(() => process.exit(0))
    .catch((error) => {
        console.error(error)
        process.exit(1)
    })