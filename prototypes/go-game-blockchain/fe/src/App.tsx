import { useRef, useState, useEffect } from "react";
import { useMoralis } from "react-moralis"
import { ConnectButton } from "web3uikit"

import './App.css';
import Board from './components/Board'
import RequestMatchButton from "./components/RequestMatchButton";
import Info from "./components/Info";
// import ConnectButton from "components/ConnectButton"; 

function App() {
  const { enableWeb3, isWeb3Enabled, Moralis, deactivateWeb3 } = useMoralis()
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
