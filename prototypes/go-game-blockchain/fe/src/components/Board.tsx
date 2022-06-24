import { useRef, useState, useEffect } from "react";
import { useMoralis, useWeb3Contract } from "react-moralis"
import { useNotification } from "web3uikit"
import { EnumType } from "typescript"

import '../App.css';
import Square from './Square'
import { contractAddresses, abi } from "../constants"

interface PlayerState {
    stoneType: EnumType;
    matchId: number;
    inGame: boolean;
};

const Board = () => {
  const [board, setBoard] = useState(Array(19).fill(Array(19).fill(null)));
  const [myMatchId, setMyMatchId] = useState(0)


  const isBlackNext = useRef(true); //black first

  // const lastRow = useRef(0);
  // const lastCol = useRef(0);

  const { chainId: chainIdHex, isWeb3EnableLoading, isWeb3Enabled } = useMoralis();
  const dispatch = useNotification();

  const chainId = 4; // parseInt(chainIdHex!); // FIXME: typescript number to index
  // const goGameAddresss = chainId in contractAddresses ? contractAddresses[chainId] : null;
  const goGameAddress = contractAddresses[chainId][0];

  /* View Functions */

  const { runContractFunction: getBoardState } = useWeb3Contract({
    abi: abi,
    contractAddress: goGameAddress, // specify the networkId
    functionName: "BoardState",
    params: {
        matchId: myMatchId
    },
  })

  const { runContractFunction: getMyPlayerState } = useWeb3Contract({
    abi: abi,
    contractAddress: goGameAddress,
    functionName: "MyPlayerState",
    params: {},
  })


  const updateBoard = (y: number, x: number, newValue: string) => {
    //copy a new board and assign the new value
    setBoard((board: Array<Array<string>>) => 
    board.map((row, currentY) => {
      if (currentY !== y) return row;
      return row.map((col, currentX) => {
        if (currentX !== x) return col;
        return newValue;
      });
    })
    );
  };

  const handlePieceClick = async (row: number, col: number, val: string) => {
    if (val) return; //return if val not null (piece already set)

    const myPlayerStateRaw: unknown = await getMyPlayerState()
    const myPlayerStateObject: PlayerState = myPlayerStateRaw as PlayerState
    const matchId = myPlayerStateObject.matchId
    console.log(`MatchId: ${matchId}`)
    setMyMatchId(matchId)

    // lastRow.current = row;
    // lastCol.current = col;

    updateBoard(row, col, isBlackNext.current ? 'black' : 'white')
    const boardState = JSON.stringify(await getBoardState({
        onError: (err) => {console.log(err)}
    })); // TODO: map contract's board state to fe
    console.log(`BoardState: ${boardState}`);

    isBlackNext.current = !isBlackNext.current; //switch turns
  }

  const handleNewNotification = () => {
    dispatch({
        type: "info",
        message: "Transaction Complete!",
        title: "Transaction Notification",
        position: "topR",
        icon: "bell",
    })
  }
  
  // Probably could add some error handling
  const handleSuccess = async (tx: any) => {
    await tx.wait(1)
    handleNewNotification()
    // updateUIValues() FIXME: update Info value
  }

  return (
    // Cell
    <div className="my-0 mx-auto">
        {board.map((row: Array<string>, rowIndex: number) => {
            return (
              <div className="flex">
                {
                  row.map((col: string, colIndex: number) => {
                    return (
                        <Square row={rowIndex} col={colIndex} val={col} onClick={handlePieceClick}/>
                    );
                  })
                }
              </div>
            );
        })}
    </div>
  );
}

export default Board;
