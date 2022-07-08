import { useRef, useState, useEffect } from "react";

import "../App.css";
import Square from "./Square";
import { useAppContext } from "context/AppContextProvider";
import useGoGameContract, { PlaceStoneParam } from "hooks/useGoGameContract";
import { useNotification } from "web3uikit";

export type StoneType = "none" | "black" | "white";
type Props = {};

const GameView = () => {
  const {
    gameState,
    playerState: { stoneType, matchId },
    metaGameState,
  } = useAppContext();
  const { runFunc: PlaceStone } = useGoGameContract({
    functionName: "PlaceStone",
  });
  const [board, setBoard] = useState<StoneType[][]>(
    Array(19).fill(Array(19).fill("none"))
  );
  const dispatch = useNotification();

  const yourStoneTypeStr =
    stoneType === 1 ? "Black" : stoneType === 2 ? "White" : undefined;
  const isMyTurn = stoneType === gameState.turn;
  const IWin = gameState.turn !== stoneType;

  useEffect(() => {
    const blockchain_board = gameState.boardState;
    setBoard((board) => {
      return board.map((row, y) => {
        return row.map((square, x) => {
          if (blockchain_board[x + 19 * y] === 2) {
            return "white";
          } else if (blockchain_board[x + 19 * y] === 1) {
            return "black";
          } else {
            return "none";
          }
        });
      }).reverse();
    });
  }, [gameState]);

  const handleSquareClick = async (
    row: number,
    col: number,
    val: StoneType
  ) => {
    if (!isMyTurn) {
      // not your turn
      console.log("not your turn")
      return;
    } else {
      console.log(`place stone ${col} ${18 - row}`);
      const placeStoneParam = {
        x: col,
        y: 18 - row,
        checkWin: true,
      };
      await PlaceStone({
        params: { params: placeStoneParam },
        onSuccess: () => {
          dispatch({
            type: "success",
            message: "place stone request sended",
            title: "Tx Completed",
            position: "topR",
          });
        },
        onError: (error) => {
          console.log(error);
        },
      });
    }
  };

  return (
    <div className="flex flex-col text-center my-[5px] mx-auto max-w-[570px]">
      <div className="flex flex-col">
        {/* Game State Dash board */}
        {metaGameState === "InGame"?
        <>
        <div>Your stone type: {yourStoneTypeStr}</div>
        <div>{isMyTurn ? "Your Turn!" : "waiting your opponent..."}</div>
        </>:
        <>
        {IWin?
      <div>You are winner!!</div>  :
      <div>You lose</div>
      }
        </>
        }
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
                    hoverVal={
                      isMyTurn
                        ? stoneType === 1
                          ? "black"
                          : "white"
                        : undefined
                    }
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
