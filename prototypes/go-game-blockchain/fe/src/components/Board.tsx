import { useRef, useState, useEffect } from "react";
import { useMoralis, useWeb3Contract } from "react-moralis"
import { useNotification } from "web3uikit"

import '../App.css';
import Square from './Square'
import { contractAddresses, abi } from "../constants"

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
  const [whosTurn, setWhosTurn] = useState('black')
  const isBlackNext = useRef(true); //black first

  // const lastRow = useRef(0);
  // const lastCol = useRef(0);

//   const { chainId: chainIdHex, isWeb3EnableLoading, isWeb3Enabled, enableWeb3 } = useMoralis();
  const dispatch = useNotification();


  const { chainId: chainIdHex, enableWeb3, isWeb3Enabled, Moralis, deactivateWeb3 } = useMoralis();

  /* View Functions */

  const { runContractFunction: getBoardState } = useWeb3Contract({
    abi: abi,
    contractAddress: goGameAddress, // specify the networkId
    functionName: "BoardState",
    params: {
        matchId: myMatchId
    },
  })

  const { runContractFunction: getWhosTurn } = useWeb3Contract({
    abi: abi,
    contractAddress: goGameAddress,
    functionName: "WhosTurn",
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

  useEffect(() => {
    const renderBoard = async () => {
        if (isWeb3Enabled) {
          await fetchBoard()
        }
    renderBoard();
    }
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

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

  const fetchBoard = async () => {
    const boardStateRaw: unknown = await getBoardState({
        onError: (err) => {console.log(err)}
    });
    const boardStateObject: string[] = boardStateRaw as string[]

    console.log(`FetchedBoardState1: ${boardStateObject}`);
    console.log(`CurrentBoardState: ${board}`);

    setBoard((board: Array<Array<string>>) => 
        board.map((row, currentY) => {
            // console.log(`row: ${row}, currentY: ${currentY}`);
            return row.map((col, currentX) => {
                const curIndex = 19 * currentY + currentX;
                if(parseInt(boardStateObject[curIndex]) === 0) return '';
                else if(parseInt(boardStateObject[curIndex]) === 1) return 'black';
                else return 'white';
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

    const currentTurn = JSON.stringify(await getWhosTurn());
    console.log(`CurrentTurn: ${currentTurn}, stone type: ${myPlayerStateObject.stoneType}`)
    if(myPlayerStateObject.stoneType === parseInt(currentTurn)) {
      updateBoard(row, col, myPlayerStateObject.stoneType === 1 ? 'black' : 'white')
      setWhosTurn(whosTurn === 'black' ? () => 'white' : 'black');
    }
    else {
      alert("It's not your turn!")
    }

    // updateBoard(row, col, myPlayerStateObject.stoneType === 1 ? 'black' : 'white')

    // await fetchBoard();
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
  
  return (
    // Cell
    <div className="my-0 mx-auto">
        <div>Turn: {whosTurn}</div>
        {board.map((row: Array<string>, rowIndex: number) => {
            return (
              <div key={rowIndex} className="flex">
                {
                  row.map((col: string, colIndex: number) => {
                    return (
                        <Square key={rowIndex + colIndex} row={rowIndex} col={colIndex} val={col} onClick={handlePieceClick}/>
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
