//SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

import "hardhat/console.sol";

library Convert {
  function addressToUint256(address a) public pure returns (uint256) {
    return uint256(uint160(a));
  }

  function addressToBytes32(address add) public pure returns (bytes32) {
    return uint256ToBytes32(addressToUint256(add));
  }

  function uint256ToBytes32(uint256 num) public pure returns (bytes32) {
    bytes memory addBytes = abi.encodePacked(num);
    bytes32 out;
    for (uint256 i = 0; i < 32; i++) {
      out |= bytes32(addBytes[i] & 0xFF) >> (i * 8);
    }
    return out;
  }
}
