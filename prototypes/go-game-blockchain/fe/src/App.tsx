import { useRef, useState, useEffect } from "react";
import './App.css';
import Square from './components/Square'
import { useMoralis } from "react-moralis"

function App() {
  const [board, setBoard] = useState(Array(19).fill(Array(19).fill(null)));
  const { enableWeb3, isWeb3Enabled, isWeb3EnableLoading, account, Moralis, deactivateWeb3 } = useMoralis()
  useEffect(() => {
    if (isWeb3Enabled) return
    if (typeof window !== "undefined") {
        if (window.localStorage.getItem("connected")) {
            enableWeb3()
            // enableWeb3({provider: window.localStorage.getItem("connected")}) // add walletconnect
        }
    }
  // eslint-disable-next-line react-hooks/exhaustive-deps
}, [isWeb3Enabled])

useEffect(() => {
  Moralis.onAccountChanged((account) => {
      console.log(`Account changed to ${account}`)
      if (account == null) {
          window.localStorage.removeItem("connected")
          deactivateWeb3()
          console.log("Null Account found")
      }
  })
  // eslint-disable-next-line react-hooks/exhaustive-deps
}, [])

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

    updateBoard(row, col, isBlackNext.current ? 'black' : 'white')

    isBlackNext.current = !isBlackNext.current; //switch turns
  }

  return (
    <div className="App">
      {/* Wrapper */}
      <div className="flex text-center my-[100px] mx-auto"> 
      <button
            onClick={async () => {
                // await walletModal.connect()
                await enableWeb3()
                // depends on what button they picked
                if (typeof window !== "undefined") {
                    window.localStorage.setItem("connected", "injected")
                    // window.localStorage.setItem("connected", "walletconnect")
                }
            }}
            disabled={isWeb3EnableLoading}
            className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded ml-auto"
        >
            Connect
        </button>
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
