// cli: yarn hardhat run scripts/requestMatch.ts --network localhost
import { ethers, getNamedAccounts } from "hardhat"

async function requestMatch() {
    const {player1, player2} = await getNamedAccounts();
    let goGame1 = await ethers.getContract("GoGame", player1)
    let goGame2 = await ethers.getContract("GoGame", player2)

    let p1 = await goGame1.MyPlayerState();
    let matchId = p1.matchId.toString();
    console.log(`matchId ${matchId}`);
    let turn = await goGame1.WhosTurn(matchId);
    let f1, f2;
    if(turn === p1.stoneType){
        f1 = goGame1;
        f2 = goGame2;
    }
    else{
        f1 = goGame2;
        f2 = goGame1;
    }

    let tx = await f1.PlaceStone(0, 0, true);
    await tx.wait(1);
    tx = await f2.PlaceStone(0, 1, true);
    await tx.wait(1);
    tx = await f1.PlaceStone(1, 0, true);
    await tx.wait(1);
    tx = await f2.PlaceStone(1, 1, true);
    await tx.wait(1);
    tx = await f1.PlaceStone(2, 0, true);
    await tx.wait(1);
    tx = await f2.PlaceStone(2, 1, true);
    await tx.wait(1);
    tx = await f1.PlaceStone(3, 0, true);
    await tx.wait(1);
    tx = await f2.PlaceStone(3, 1, true);
    await tx.wait(1);
    console.log("Place stone done")

}

requestMatch()
    .then(() => process.exit(0))
    .catch((error) => {
        console.error(error)
        process.exit(1)
    })