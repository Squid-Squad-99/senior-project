import { useRef, useState, useEffect } from "react";

import "../App.css";
import Square from "./Square";
import { useAppContext } from "context/AppContextProvider";
 
export type StoneType = "none" | "black" | "white";
type Props = {

};

const GameView = () => {
  const { gameState, playerState: {stoneType}, metaGameState } = useAppContext();
  const [board, setBoard] = useState<StoneType[][]>(Array(19).fill(Array(19).fill("none")));

  
  const yourStoneTypeStr = stoneType === 1 ? "Black": stoneType === 2? "White": undefined;

  const updateBoard = (y: number, x: number, newValue: StoneType) => {
    //copy a new board and assign the new value
    setBoard((board: Array<Array<StoneType>>) =>
      board?.map((row, currentY) => {
        if (currentY !== y) return row;
        return row?.map((col, currentX) => {
          if (currentX !== x) return col;
          return newValue;
        });
      })
    );
  };

  const handleSquareClick = (row: number, col: number, val: StoneType) => {
    console.log("square click")
  };

  return (
    <div className="flex flex-col text-center my-[5px] mx-auto max-w-[570px]">
      <div className="flex">
        {/* Game State Dash board */}
        <div>Your stone type: {playerState.stoneType}</div>
      </div>

      {/* Board */}
      <div className="my-0 mx-auto">
        {board?.map((row, rowIndex) => {
          return (
            <div key={rowIndex} className="flex">
              {row?.map((item, colIndex) => {
                return (
                  <Square
                    key={rowIndex + colIndex}
                    row={rowIndex}
                    col={colIndex}
                    val={item}
                    onClick={handleSquareClick}
                    hoverVal={undefined}
                  />
                );
              })}
            </div>
          );
        })}
      </div>
    </div>
  );
};

export default GameView;
