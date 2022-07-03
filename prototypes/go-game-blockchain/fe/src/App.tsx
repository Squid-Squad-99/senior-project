import { useEffect, useState } from "react"
import { useMoralis, useMoralisQuery, useMoralisSubscription, useWeb3Contract } from 'react-moralis'

import './App.css';
import networkMapping from './constants/networkMapping.json'
import abi from './constants/abi.json'
import Header from 'components/Header';
import Board from 'components/Board';
import BoardStatus from 'components/BoardStatus'

interface PlayerState {
  stoneType: number;
  matchId: number;
  inGame: boolean;
};

function App() {
  const { chainId, account, isWeb3Enabled } = useMoralis()
  const chainString = chainId ? parseInt(chainId).toString() : "31337"
  const goGameAddress = (networkMapping as any)[chainString].GoGame[0]

  const [myMatchId, setMyMatchId] = useState("0")
  const [myStoneType, setMyStoneType] = useState("")
  const [whosTurn, setWhosTurn] = useState("")
  const [gameStateChanged, setGameStateChanged] = useState(false)

  const { runContractFunction: getMyPlayerState } = useWeb3Contract({
    abi: abi,
    contractAddress: goGameAddress,
    functionName: "MyPlayerState",
    params: {},
  })

  const { runContractFunction: getWhosTurn } = useWeb3Contract({
    abi: abi,
    contractAddress: goGameAddress,
    functionName: "WhosTurn",
    params: {
        matchId: myMatchId
    },
})

  // query => query.equalTo("matchId", myMatchId)
  useMoralisSubscription("GameOver", q => q, [], {
    onCreate: data => {
      alert(`Game Over! (matchID: ${data.attributes.matchId}, winner: ${data.attributes.winner})`)
    },
    onUpdate: data => {
      alert(`Game Over! (matchID: ${data.attributes.matchId}, winner: ${data.attributes.winner})`)
    },
  });

  useMoralisSubscription("GameStateChange", q => q, [], {
    onCreate: data => {
      // alert(`game: ${data.attributes.matchId}'s state created`)
      console.log(`game: ${data.attributes.matchId}'s state created`)
      setGameStateChanged(true)
    },
    onUpdate: data => {
      // alert(`game: ${data.attributes.matchId}'s state updated`)
      console.log(`game: ${data.attributes.matchId}'s state updated`)
      setGameStateChanged(true)
    },
  });

  const { data: findMatchPlayer1, isFetching: fetchingFindMatchPlayer1 } = useMoralisQuery(
    "FindMatch",
    query => query.equalTo("player1", account?.toString()),
    [],
    {
      live: true,
    }
  )
  console.log(findMatchPlayer1)

  const { data: findMatchPlayer2, isFetching: fetchingFindMatchPlayer2 } = useMoralisQuery(
    "FindMatch",
    query => query.equalTo("player2", account?.toString()),
    [],
    {
      live: true,
    }
  )
  console.log(findMatchPlayer2)

  async function setupUI() {
    const myPlayerStateRaw: unknown = await getMyPlayerState()
    const myPlayerStateObject: PlayerState = myPlayerStateRaw as PlayerState
    setMyMatchId(myPlayerStateObject?.matchId?.toString())
    setMyStoneType(myPlayerStateObject?.stoneType?.toString() === "1" ? "black" : "white")
    console.log(`MatchId: ${myPlayerStateObject?.matchId}`)
    console.log(`StoneType: ${myPlayerStateObject?.stoneType}`)

    const whosturnRaw: unknown = await getWhosTurn()
    const whosTurnStr: string = whosturnRaw as string
    console.log(`Whosturn: ${whosTurnStr}`)
    setWhosTurn(whosTurnStr?.toString() === "1" ? "black" : "white")
  }

  useEffect(() => {
    setupUI()
    setGameStateChanged(false)
  }, [chainId, account, isWeb3Enabled, findMatchPlayer1, findMatchPlayer2, whosTurn, gameStateChanged])

  return (
    <div className="App">
      <Header goGameAddress={goGameAddress} myMatchId={myMatchId}/>
      <hr />
      
      <div className="h-12 mt-5 flex flex-col space-y-0 justify-center mx-auto max-w-[570px]">
        {isWeb3Enabled ? (
            (((fetchingFindMatchPlayer1 || fetchingFindMatchPlayer2) 
            && <div className="mx-2"> Fetching data... </div>) 
            || <BoardStatus myMatchId={myMatchId} myStoneType={myStoneType} whosTurn={whosTurn}/>
          )
          || (
            (findMatchPlayer1.length === 0 && findMatchPlayer2.length === 0 && myMatchId === "0") && (<div className="mx-2"> Finding a match... </div>)
          )) : <div className="mx-2">Web3 Currently Not Enabled</div>
        }
      </div>

      <Board 
        goGameAddress={goGameAddress} 
        whosTurn={whosTurn}
        myStoneType={myStoneType}
        myMatchId={myMatchId}/>
    </div>
  );
}

export default App;
