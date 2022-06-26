

import { ethers, network } from "hardhat"

async function requestMatch() {
    const goGame = await ethers.getContract("GoGame")
    await goGame.requestMatch()
    const tx = await goGame.requestMatch()
    await tx.wait(1)
    console.log("Match requested!")
    // if ((network.config.chainId = "31337")) {
    //     await moveBlocks(2, (sleepAmount = 1000))
    // }
}

requestMatch()
    .then(() => process.exit(0))
    .catch((error) => {
        console.error(error)
        process.exit(1)
    })