import { useEffect, useState } from "react"
import { useMoralis, useMoralisQuery, useMoralisSubscription, useWeb3Contract } from 'react-moralis'
import './App.css';
import networkMapping from './constants/networkMapping.json'
import abi from './constants/abi.json'
import Header from 'components/Header';
import Board from 'components/Board';

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
  const [myStoneType, setMyStoneType] = useState("0")
  const [whosTurn, setWhosTurn] = useState("0")
  const [isGameOver, setIsGameOver] = useState(false)


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
      setIsGameOver(true)
      alert(`Game Over! (matchID: ${data.attributes.matchId}, winner: ${data.attributes.winner})`)
    },
    onUpdate: data => {
      setIsGameOver(true)
      alert(`Game Over! (matchID: ${data.attributes.matchId}, winner: ${data.attributes.winner})`)
    },
  });

  useMoralisSubscription("GameStateChange", q => q, [], {
    onCreate: data => alert(`game: ${data.attributes.matchId}'s state changed`),
    onUpdate: data => alert(`game: ${data.attributes.matchId}'s state updated`),
  });

  const { data: gameStateChangeData , isFetching } = useMoralisQuery(
    "GameStateChange",
    query => query.equalTo("matchId", myMatchId),
    [],
    {
      live: true,
    }
  )

  // FIXME: only trigger when find match event emitted
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
    setMyStoneType(myPlayerStateObject?.stoneType?.toString())
    console.log(`MatchId: ${myPlayerStateObject?.matchId}`)
    console.log(`StoneType: ${myPlayerStateObject?.stoneType}`)

    const whosturnRaw: unknown = await getWhosTurn()
    const whosTurnStr: string = whosturnRaw as string
    setWhosTurn(whosTurnStr)
    console.log(`Whosturn: ${whosTurnStr}`)
  }

  useEffect(() => {
    setupUI()
  }, [chainId, account, isWeb3Enabled, findMatchPlayer1, findMatchPlayer2, whosTurn])

  // useEffect(() => {
  //   if(isGameOver) {
  //     alert("Game Over!")
  //     console.log("Game Over!")
  //     setIsGameOver(false)
  //   }
  // }, [isGameOver])

  return (
    <div className="App">
      <Header goGameAddress={goGameAddress}/>
      <hr />
      <div className="mt-2.5">My matchID: {myMatchId}</div>
      <div>My stone type: {myStoneType}</div>
      <div>Whos turn now: {whosTurn}</div>
      {isWeb3Enabled ? (
          ((fetchingFindMatchPlayer1 || fetchingFindMatchPlayer2) && <div className="mx-2"> Fetching data... </div>)
         || (
          (findMatchPlayer1.length === 0 && findMatchPlayer2.length === 0 && myMatchId === "0") && (<div className="mx-2"> Finding a match... </div>)
        )) : <div className="mx-2">Web3 Currently Not Enabled</div>
      }
      <Board 
        goGameAddress={goGameAddress} 
        whosTurn={whosTurn}
        myStoneType={myStoneType}
        myMatchId={myMatchId}/>
    </div>
  );
}

export default App;
