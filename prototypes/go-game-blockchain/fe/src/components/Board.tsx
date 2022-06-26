import { useRef, useState, useEffect } from "react";
import { useMoralis, useWeb3Contract } from "react-moralis"

import '../App.css';
import Square from './Square'


const Board = () => {
    const [board, setBoard] = useState(Array(19).fill(Array(19).fill(null)));

    const isBlackNext = useRef(true); //black first
  
    // const lastRow = useRef(0);
    // const lastCol = useRef(0);
  
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
  
    const handlePieceClick = (row: number, col: number, val: string) => {
      if (val) return; //return if val not null (piece already set)
  
      // lastRow.current = row;
      // lastCol.current = col;
  
      updateBoard(row, col, isBlackNext.current ? "black" : "white");
  
      isBlackNext.current = !isBlackNext.current; //switch turns
    };

    return (
        <div className="flex text-center my-[60px] mx-auto">
            <div className='my-0 mx-auto'>
                {board.map((row: Array<string>, rowIndex: number) => {
                return (
                    <div className='flex'>
                    {row.map((col: string, colIndex: number) => {
                        return (
                        <Square
                            row={rowIndex}
                            col={colIndex}
                            val={col}
                            onClick={handlePieceClick}
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
