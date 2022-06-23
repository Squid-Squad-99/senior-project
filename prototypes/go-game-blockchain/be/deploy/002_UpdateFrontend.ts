import {ethers, network} from "hardhat"
import {HardhatRuntimeEnvironment} from 'hardhat/types'
import {DeployFunction} from 'hardhat-deploy/types'

import fs from "fs"

// var contractAddress = "0xC31360bf7d1b78F79B39996801BaE0C2CeC81FBB";
// var FE_ABI_FILE = "../../fe/src/constants/abi.json"
// var d = new AbiInterfaceObject();
// d.copyInto(FE_ABI_FILE);

const FRONTEND_ADDRESS_FILE = "../../fe/src/constants/contractAddress.json"
const FRONTEND_ABI_FILE = "../../fe/src/constants/abi.json"

const func: DeployFunction = async function (hre: HardhatRuntimeEnvironment) {
  console.log("updating fe...");
  updateContractAddresses();
};

async function updateContractAddresses() {
    console.log("here1");



    let addressData = fs.readFile(FRONTEND_ADDRESS_FILE, 'utf8', function(err, data){
      
        // Display the file content
        console.log(data);
    });
    console.log("read end");
    console.log(addressData);
      

    let contractAddresses = JSON.parse(fs.readFileSync(FRONTEND_ADDRESS_FILE, "utf8"));
    let contractAbi = JSON.parse(fs.readFileSync(FRONTEND_ABI_FILE, "utf8"));
    console.log("here3");
    // console.log(`${contractAddresses}`);

    const goGame = await ethers.getContract(contractAddresses, contractAbi);
    console.log(`here2`);

    // const contractAddresses = JSON.parse(fs.readFileSync(FRONTEND_ADDRESS_FILE, "utf8"));
    // console.log(`chainID is ${network.config.chainId}`);
    // if (network.config.chainId!.toString() in contractAddresses) {
    //     if (!contractAddresses[network.config.chainId!.toString()].includes(goGame.address)) {
    //         contractAddresses[network.config.chainId!.toString()].push(goGame.address)
    //     }
    // } else {
    //     contractAddresses[network.config.chainId!.toString()] = [goGame.address]
    // }
    // fs.writeFileSync(FRONTEND_ADDRESS_FILE, JSON.stringify(contractAddresses))
    // console.log("here3");

}

export default func;
func.tags = ['All', 'Frontend']