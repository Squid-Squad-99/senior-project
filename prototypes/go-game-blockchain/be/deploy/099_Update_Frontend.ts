import {ethers, network} from 'hardhat'
import {HardhatRuntimeEnvironment} from 'hardhat/types';
import {DeployFunction} from 'hardhat-deploy/types';
import fs from 'fs'


const frontendContractsFile = "../fe/src/constants/networkMapping.json"

async function updateContractAddresses (hre: HardhatRuntimeEnvironment) {
    const goGame = await ethers.getContract("GoGame")
    const { getChainId } = hre
    const chainId = await getChainId()
    const contractAddresses = JSON.parse(fs.readFileSync(frontendContractsFile, "utf8"))
    if(chainId in contractAddresses) {
        if(!contractAddresses[chainId]["GoGame"].includes(goGame.address)) {
            contractAddresses[chainId]["GoGame"].push(goGame.address)
        }
    }
    else {
        contractAddresses[chainId] = {"GoGame": [goGame.address]}
    }
    fs.writeFileSync(frontendContractsFile, JSON.stringify(contractAddresses))
};

const func: DeployFunction = async function (hre: HardhatRuntimeEnvironment) {
    if(process.env.UPDATE_FRONTEND) {
        console.log("updating fe...")
        await updateContractAddresses(hre)
    }
};

export default func;
func.tags = ['All', 'frontend']