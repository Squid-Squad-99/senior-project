import { useWeb3Contract } from "react-moralis"
import { contractAddresses, abi } from "../constants"
// dont export from moralis when using react
import { useMoralis } from "react-moralis"
import { useEffect, useState } from "react"
import { useNotification } from "web3uikit"

interface PlayerState {
  stoneType: number;
  matchId: number;
  inGame: boolean;
};
  
const RequestMatchButton = () => {
  const [myMatchId, setMyMatchId] = useState('0')
  const { chainId: chainIdHex, isWeb3EnableLoading, isWeb3Enabled } = useMoralis(); // FIXME: need automated enableweb3 || authentication
  const chainId = 4 // chainIdHex ? parseInt(chainIdHex) : null;

  const goGameAddress = contractAddresses[chainId][0];

  const dispatch = useNotification();

  const { runContractFunction: getMyPlayerState } = useWeb3Contract({
    abi: abi,
    contractAddress: goGameAddress,
    functionName: "MyPlayerState",
    params: {},
  })


  const {
    runContractFunction: requestMatch,
    data: enterTxResponse,
    isLoading,
    isFetching
  } = useWeb3Contract({
    abi: abi,
    contractAddress: goGameAddress,
    functionName: 'requestMatch',
    params: {},
  });

  const handleNewNotification = () => {
    dispatch({
      type: "info",
      message: "Transaction Complete!",
      title: "Transaction Notification",
      position: "topR",
      icon: "bell",
    })
  }

  async function updateMatch() {
    const myPlayerStateRaw: unknown = await getMyPlayerState()
    const myPlayerStateObject: PlayerState = myPlayerStateRaw as PlayerState
    const matchId = myPlayerStateObject.matchId
    console.log(`MatchId: ${matchId}`)
    setMyMatchId(matchId.toString())

  }

  useEffect(() => {
    if (isWeb3Enabled && myMatchId !== '0') {
        updateMatch()
    }
  }, [isWeb3Enabled])

  const handleSuccess = async (tx: any) => {
      await tx.wait(1)
      updateMatch()
      handleNewNotification()

  }

  const handleOnClick = async () => {
    await requestMatch({
        onSuccess: handleSuccess,
        onError: (error) => console.log(error),
    })
    console.log(`request match`);
  }


  return (
    // Cell
    <button 
      className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded ml-auto"
      onClick={handleOnClick}
      disabled={isWeb3EnableLoading || !chainId || isLoading || isFetching || myMatchId !== '0'}
    >
     {myMatchId === '0' ? `Request Match` : `Matched! (ID: ${myMatchId})`}
    </button>
  );
};

export default RequestMatchButton;