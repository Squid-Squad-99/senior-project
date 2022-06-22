import * as dotenv from "dotenv";

import { HardhatUserConfig, task } from "hardhat/config";
import "@nomiclabs/hardhat-etherscan";
import "@nomiclabs/hardhat-waffle";
import "@typechain/hardhat";
import "hardhat-gas-reporter";
import "solidity-coverage";
import 'hardhat-deploy';
import '@nomiclabs/hardhat-ethers';

dotenv.config();

// This is a sample Hardhat task. To learn how to create your own go to
// https://hardhat.org/guides/create-task.html
task("accounts", "Prints the list of accounts", async (taskArgs, hre) => {
  const accounts = await hre.ethers.getSigners();

  for (const account of accounts) {
    console.log(account.address);
  }
});

// You need to export an object to set up your config
// Go to https://hardhat.org/config/ to learn more

const config: HardhatUserConfig = {
  solidity: "0.8.4",
  networks: {
    rinkeby: {
      url: process.env.RINKEBY_URL || "",
      accounts: [
        process.env.PRIVATE_KEY_1 !== undefined ? process.env.PRIVATE_KEY_1: "",
        process.env.PRIVATE_KEY_2 !== undefined ? process.env.PRIVATE_KEY_2: "",
        process.env.PRIVATE_KEY_3 !== undefined ? process.env.PRIVATE_KEY_3: "",
      ],
      chainId: 4,
      gas: 30000000,
      gasPrice: 2000000000,
    },
    localhost: {
      url: "http://127.0.0.1:8545/",
      chainId: 31337,
      gas: 30000000,
    },
  },
  namedAccounts: {
    deployer: 0,
    player1: 1,
    player2: 2,
  },

  gasReporter: {
    enabled: process.env.REPORT_GAS !== undefined,
    currency: "USD",
  },
  etherscan: {
    apiKey: process.env.ETHERSCAN_API_KEY,
  },
  mocha: {
    timeout: 600000,
  }
};

export default config;
