import {HardhatRuntimeEnvironment} from 'hardhat/types';
import {DeployFunction} from 'hardhat-deploy/types';
import { ethers } from 'hardhat';

const func: DeployFunction = async function (hre: HardhatRuntimeEnvironment) {
  const {deployments, getNamedAccounts} = hre;
  const {deploy} = deployments;

   const {deployer} = await getNamedAccounts();

   // deploy
   const converterLibrary = await deploy("Convert", {
     from: deployer,
     log: true
   });
   
   await deploy("GoGame", {
    from: deployer,
    log: true,
    libraries: {
      "Convert":  converterLibrary.address
    }
});
};
export default func;
func.tags = ['All', 'GoGame']