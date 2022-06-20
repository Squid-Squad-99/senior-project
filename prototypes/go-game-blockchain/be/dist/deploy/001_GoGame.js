"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const func = async function (hre) {
    const { deployments, getNamedAccounts } = hre;
    const { deploy } = deployments;
    const { deployer } = await getNamedAccounts();
    // deploy
    await deploy("GoGame", {
        from: deployer,
        log: true,
    });
};
exports.default = func;
func.tags = ['All', 'GoGame'];
