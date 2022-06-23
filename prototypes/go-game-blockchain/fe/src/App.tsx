import { useRef, useState } from "react";
import "./App.css";
import Square from "./components/Square";

function App() {
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
    <div className='App'>
      {/* Wrapper */}
      <div className='flex text-center my-[100px] mx-auto'>
        {/* Board */}
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
    </div>
  );
}

export default App;
