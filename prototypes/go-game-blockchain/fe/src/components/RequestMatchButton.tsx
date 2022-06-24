import { useWeb3Contract } from "react-moralis"
import { contractAddresses, abi } from "../constants"
// dont export from moralis when using react
import { useMoralis } from "react-moralis"
import { useEffect, useState } from "react"
import { useNotification } from "web3uikit"
import { ethers } from "ethers"
import { assert } from "console"

type Props = {
    text?: string;
}

interface ContractAddressObj {
  chainId: number
  address: string
}
  
const RequestMatchButton = ({text}: Props) => {
  const { chainId: chainIdHex, isWeb3EnableLoading } = useMoralis(); // FIXME: need automated enableweb3 || authentication
  const chainId = 4 // chainIdHex ? parseInt(chainIdHex) : null;
  // const raffleAddress = chainId in contractAddresses ? contractAddresses[chainId] : null;
  // let myAddress:  { string: ContractAddressObj[] } = JSON.parse(contractAddresses.toString());
  // console.log(myAddress);
  const goGameAddress = contractAddresses[chainId][0];

  const {runContractFunction: requestMatch} = useWeb3Contract({
    abi: abi,
    contractAddress: goGameAddress,
    functionName: 'requestMatch',
    params: {},
  });

  const handleOnClick = async () => {
    await requestMatch({
        //onSuccess: handleSuccess,
        onError: (error) => console.log(error),
    })
    console.log(`request match`);
  }


  return (
    // Cell
    <button 
      className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded ml-auto"
      onClick={handleOnClick}
      disabled={isWeb3EnableLoading || !chainId}
    >
      {chainId ? `${text}` : "invalid chain!"}
    </button>
  );
};

export default RequestMatchButton;