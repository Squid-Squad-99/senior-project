import { useRef, useState } from "react";
import './App.css';
import Square from './components/Square'

function App() {
  const [board, setBoard] = useState(Array(19).fill(Array(19).fill(null)));

  const isBlackNext = useRef(true);
  const lastRow = useRef(0);
  const lastCol = useRef(0);

  const updateBoard = (y: number, x: number, newValue: string) => {
    // 有兩種可能性：沒有被點擊到的列 + 被點擊到的那一列
    setBoard((board: Array<Array<string>>) =>
    // 如果沒有點擊到這一列裡面的任一個欄，那就直接回傳這一列，沒它的事
    board.map((row, currentY) => {
      if (currentY !== y) return row;
      // 如果被點擊到了，找出這一列裡面被點到的那一欄，回傳它的值
      return row.map((col, currentX) => {
        if (currentX !== x) return col;
        return newValue;
      });
    })
    );
  };

  const handlePieceClick = (row: number, col: number, val: string) => {
    if (val) return;
    lastRow.current = row;
    lastCol.current = col;
    updateBoard(row, col, isBlackNext.current ? 'black' : 'white')
    isBlackNext.current = !isBlackNext.current;
    console.log(`hi from row ${row} col ${col}`)
  }

  return (
    <div className="App">
      {/* Wrapper */}
      <div className="flex text-center my-[100px] mx-auto"> 
        {/* Board */}
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
      </div>
    </div>
  );
}

export default App;
