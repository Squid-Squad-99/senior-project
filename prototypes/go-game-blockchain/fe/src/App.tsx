import { useRef, useState, useEffect } from "react";
import { useMoralis, useWeb3Contract } from "react-moralis"
import { ConnectButton } from "web3uikit"

import './App.css';
import Board from './components/Board'
import RequestMatchButton from "./components/RequestMatchButton";
import Info from "./components/Info";
import { contractAddresses, abi } from  './constants'

interface PlayerState {
  stoneType: number;
  matchId: number;
  inGame: boolean;
};

function App() {
  const { enableWeb3, isWeb3Enabled, Moralis, deactivateWeb3 } = useMoralis()
  const [myStoneType, setMyStoneType] = useState(0)

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

const chainId = 4;
const goGameAddress = contractAddresses[chainId][0];
const { runContractFunction: getMyPlayerState } = useWeb3Contract({
  abi: abi,
  contractAddress: goGameAddress,
  functionName: "MyPlayerState",
  params: {},
})

const updatePlayerState = async () => {
  const myPlayerStateRaw: unknown = await getMyPlayerState()
  const myPlayerStateObject: PlayerState = myPlayerStateRaw as PlayerState
  setMyStoneType(myPlayerStateObject.stoneType)
}

useEffect(() => {
  Moralis.onAccountChanged((account) => {
      console.log(`Account changed to ${account}`)
      if (account == null) {
          window.localStorage.removeItem("connected")
          deactivateWeb3()
          console.log("Null Account found")
      }
      else {
        updatePlayerState()
        console.log("player stone type should be updated")
      }
  })
  // eslint-disable-next-line react-hooks/exhaustive-deps
}, [])

  return (
    <div className="App">
      <div className="flex">
        <Info />
        <RequestMatchButton />
        <ConnectButton moralisAuth={false}/>
      </div>
      {/* Wrapper */}
      <div className="flex text-center my-[100px] mx-auto"> 
        <Board />
      </div>
    </div>
  );
}

export default App;
