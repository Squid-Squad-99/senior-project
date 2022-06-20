//SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

import "hardhat/console.sol";

contract GoGame {

    event FindMatch(address indexed player1, address indexed player2);

    address[] matchQueue;

    function requestMatch() public {
        matchQueue.push(msg.sender);

        if(matchQueue.length == 2){
            emit FindMatch(matchQueue[0], matchQueue[1]);
            console.log("Find match!!", matchQueue[0], matchQueue[1]);
        }
    }

    function hello() public pure returns (string memory) {
        return "Hello you";
    }

    constructor() {

    }
}