import { useRef, useState, useEffect } from "react";
import { useMoralis, useWeb3Contract, useMoralisQuery } from "react-moralis"
import abi from '../constants/abi.json'

import '../App.css';
import Square from './Square'

type Props = {
  goGameAddress: string;
  myTurn: boolean;
  myMatchId: string;
}

const Board = (props: Props) => {
    const [board, setBoard] = useState(Array(19).fill(Array(19).fill(null)));

    const isBlackNext = useRef(true); //black first
  
    // const lastRow = useRef(0);
    // const lastCol = useRef(0);

    const { data: gameStateChangeData , isFetching } = useMoralisQuery(
      "GameStateChange",
      query => query.equalTo("matchId", props.myMatchId),
      [],
      {
        live: true,
      }
    )

    const { runContractFunction: getBoardState } = useWeb3Contract({
      abi: abi,
      contractAddress: props.goGameAddress, // specify the networkId
      functionName: "BoardState",
      params: {
          matchId: props.myMatchId
      },
    })
  
    const updateBoard = (y: number, x: number, newValue: string) => {
      //copy a new board and assign the new value
      setBoard((board: Array<Array<string>>) =>
        board?.map((row, currentY) => {
          if (currentY !== y) return row;
          return row?.map((col, currentX) => {
            if (currentX !== x) return col;
            return newValue;
          });
        })
      );
    };
  
    const handlePieceClick = (row: number, col: number, val: string) => {
      if (val) return; //return if val not null (piece already set)
      if (!props.myTurn) alert("It is not your turn!")
      else {
        updateBoard(row, col, isBlackNext.current ? "black" : "white");
        isBlackNext.current = !isBlackNext.current; //switch turns
      }  
    };

    const fetchBoard = async () => {
      const boardStateRaw: unknown = await getBoardState({
          onError: (err) => {console.log(err)}
      });
      const boardStateObject: string[] = boardStateRaw as string[]
  
      console.log(`FetchedBoardState1: ${boardStateObject}`);
      console.log(`CurrentBoardState: ${board}`);

      if(boardStateObject !== undefined) {
        setBoard((board: Array<Array<string>>) => 
          board?.map((row, currentY) => {
              // console.log(`row: ${row}, currentY: ${currentY}`);
              return row?.map((col, currentX) => {
                  const curIndex = 19 * currentY + currentX;
                  if(parseInt(boardStateObject[curIndex]) === 0) return '';
                  else if(parseInt(boardStateObject[curIndex]) === 1) return 'black';
                  else return 'white';
              });
          })
        );
      }
    };

    useEffect(() => {
      fetchBoard()
    }, [gameStateChangeData])

    return (
        <div className="flex text-center my-[20px] mx-auto">
            <div className='my-0 mx-auto'>
                {board?.map((row: Array<string>, rowIndex: number) => {
                return (
                    <div key={rowIndex} className='flex'>
                    {row?.map((col: string, colIndex: number) => {
                        return (
                        <Square
                            key={rowIndex + colIndex}
                            row={rowIndex}
                            col={colIndex}
                            val={col}
                            onClick={handlePieceClick}
                            goGameAddress={props.goGameAddress}
                        />
                        );
                    })}
                    </div>
                );
                })}
            </div>
        </div>
      );
  
}

export default Board;
