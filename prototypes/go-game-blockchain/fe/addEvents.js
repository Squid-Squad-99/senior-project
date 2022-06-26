// Only get success response when database table modified
// cli: node addEvents.ts
const Moralis = require("moralis/node");

require("dotenv").config();
const contractAddresses = require("./src/constants/networkMapping.json");
let chainId = process.env.chainId || 31337;
let moralisChainId = chainId == "31337" ? "1337" : chainId;
const contractAddress = contractAddresses[chainId]["GoGame"][0];

const serverUrl = process.env.REACT_APP_MORALIS_SERVER_URL;
const appId = process.env.REACT_APP_MORALIS_APP_ID;
const masterKey = process.env.masterKey;

async function main() {
  await Moralis.start({ serverUrl, appId, masterKey });
  console.log(`working with contract address ${contractAddress}`);

  let findMatchOptions = {
    chainId: moralisChainId,
    sync_historical: true,
    topic: "FindMatch(uint256, address, address)",
    address: contractAddress,
    abi: {
      anonymous: false,
      inputs: [
        {
          indexed: true,
          internalType: "uint256",
          name: "matchId",
          type: "uint256",
        },
        {
          indexed: false,
          internalType: "address",
          name: "player1",
          type: "address",
        },
        {
          indexed: false,
          internalType: "address",
          name: "player2",
          type: "address",
        },
      ],
      name: "FindMatch",
      type: "event",
    },
    tableName: "FindMatch",
  };

  let gameStateChangeOptions = {
    chainId: moralisChainId,
    sync_historical: true,
    topic: "GameStateChange(uint256)",
    address: contractAddress,
    abi: {
      anonymous: false,
      inputs: [
        {
          indexed: true,
          internalType: "uint256",
          name: "matchId",
          type: "uint256",
        },
      ],
      name: "GameStateChange",
      type: "event",
    },
    tableName: "GameStateChange",
  };

  let gameOverOptions = {
    chainId: moralisChainId,
    sync_historical: true,
    topic: "GameOver(uint256, address)",
    address: contractAddress,
    abi: {
      anonymous: false,
      inputs: [
        {
          indexed: true,
          internalType: "uint256",
          name: "matchId",
          type: "uint256",
        },
        {
          indexed: false,
          internalType: "address",
          name: "winner",
          type: "address",
        },
      ],
      name: "GameOver",
      type: "event",
    },
    tableName: "GameOver",
  };

  const findMatchResponse = await Moralis.Cloud.run(
    "watchContractEvent",
    findMatchOptions,
    {
      useMasterKey: true,
    }
  );
  const gameStateChangeResponse = await Moralis.Cloud.run(
    "watchContractEvent",
    gameStateChangeOptions,
    {
      useMasterKey: true,
    }
  );
  const gameOverResponse = await Moralis.Cloud.run(
    "watchContractEvent",
    gameOverOptions,
    {
      useMasterKey: true,
    }
  );
  // console.log(findMatchResponse.success);
  // console.log(gameStateChangeResponse.success);
  // console.log(gameOverResponse.success);

  if (
    findMatchResponse.success &&
    gameStateChangeResponse.success &&
    gameOverResponse.success
  ) {
    console.log("Success! Database Updated with watching events");
  } else {
    console.log("Something went wrong...");
  }
}

main()
  .then(() => process.exit(0))
  .catch((error) => {
    console.error(error);
    process.exit(1);
  });
