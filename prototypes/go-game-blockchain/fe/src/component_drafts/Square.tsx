import { useWeb3Contract } from "react-moralis"
import { contractAddresses, abi } from "../constants"
// dont export from moralis when using react
import { useMoralis } from "react-moralis"
import { useEffect, useState } from "react"
import { useNotification } from "web3uikit"
import { ethers } from "ethers"

type Props = {
  row?: number;
  col?: number;
  val?: string;
  onClick: Function;
}

const Square = (props: Props) => {
  const beforeStyle = `before:content-[''] before:w-[2px] before:bg-black before:absolute before:left-1/2 before:-translate-x-1/2 ${props.row === 0 ? 'before:top-1/2' : 'before:top-0'} ${props.row === 18 ? 'before:h-1/2' : 'before:h-full'} `
  const afterStyle = `after:content-[''] after:h-[2px] after:bg-black after:absolute after:top-1/2 after:-translate-y-1/2 ${props.col === 0 ? 'after:left-1/2' : 'after:left-0'} ${props.col === 18 ? 'after:w-1/2' : 'after:w-full'} `

  const [updateInfo, setUpdateInfo] = useState(false);

  const { chainId: chainIdHex, isWeb3EnableLoading } = useMoralis();
  const dispatch = useNotification()

  const chainId = 4; // parseInt(chainIdHex!); // FIXME: typescript number to index
  // const goGameAddress = chainId in contractAddresses ? contractAddresses[chainId] : null;
  const goGameAddress = contractAddresses[chainId][0];

  const options = {
    abi: abi,
    contractAddress: goGameAddress,
    functionName: 'PlaceStone',
    params: {
      x: props.col,
      y: props.row,
      checkWin: true
    }
  };
  const {data, error, runContractFunction: placeStone, isFetching, isLoading} = useWeb3Contract(options);


  const handleNewNotification = () => {
    dispatch({
        type: "info",
        message: "Transaction Complete!",
        title: "Transaction Notification",
        position: "topR",
        icon: "bell",
    })
}

  const handleSuccess = async (tx: any) => {
    await tx.wait(1)
    console.log(`place stone tx: ${tx.address}`) // TODO: get tx address
    handleNewNotification()
    console.log("stone placed!")
    console.log(props.row, props.col, props.val)
  }
  const handleSquareClick = async () => {
    props.onClick(props.row, props.col, props.val)
    await placeStone({
      onSuccess: handleSuccess,
      onError: (error) => {console.log(error)},
    });
  }
  return (
    // Cell
    <div className={'bg-[#ba8c63] w-[30px] h-[30px] relative ' + beforeStyle + afterStyle}>
      {/* Piece */}
      <div className={`w-full h-full rounded-[50%] absolute scale-50 z-50 ${props.val === 'white' ? 'bg-white' : ''} ${props.val === 'black' ? 'bg-black' : ''}`} onClick={handleSquareClick}/>
    </div>
  );
};

export default Square;