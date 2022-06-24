import { useRef, useState } from "react";
import { useMoralis, useWeb3Contract } from "react-moralis"
import { useNotification } from "web3uikit"

import '../App.css';
import Square from './Square'
import { contractAddresses, abi } from "../constants"
import { info } from "console";

interface PlayerState {
    stoneType: number;
    matchId: number;
    inGame: boolean;
};

const Board = () => {
  const chainId = 4; // parseInt(chainIdHex!); // FIXME: typescript number to index
  // const goGameAddresss = chainId in contractAddresses ? contractAddresses[chainId] : null;
  const goGameAddress = contractAddresses[chainId][0];

  const [board, setBoard] = useState(Array(19).fill(Array(19).fill(null)));
  const [myMatchId, setMyMatchId] = useState(0)
  const isBlackNext = useRef(true); //black first


  // const lastRow = useRef(0);
  // const lastCol = useRef(0);

  const { chainId: chainIdHex, isWeb3EnableLoading, isWeb3Enabled } = useMoralis();
  const dispatch = useNotification();

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

    // updateBoard(row, col, isBlackNext.current ? 'black' : 'white')
    const boardStateRaw: unknown = await getBoardState({
        onError: (err) => {console.log(err)}
    });
    const boardStateObject: string[] = boardStateRaw as string[]

    console.log(`FetchedBoardState1: ${boardStateObject}`);
    console.log(`CurrentBoardState: ${board}`);

    setBoard((board: Array<Array<string>>) => 
        board.map((row, currentY) => {
            console.log(`row: ${row}, currentY: ${currentY}`);
            return row.map((col, currentX) => {
                const curIndex = 19 * currentY + currentX;
                if(parseInt(boardStateObject[curIndex]) === 0) return '';
                else if(parseInt(boardStateObject[curIndex]) === 1) return 'black';
                else return 'white';
            });
        })
    );
    updateBoard(row, col, myPlayerStateObject.stoneType === 1 ? 'black' : 'white')
    // TODO: not your turn alert

    console.log(`CurrentBoardState2: ${board}`);


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
