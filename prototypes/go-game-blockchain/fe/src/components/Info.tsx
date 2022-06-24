import { useWeb3Contract } from "react-moralis"
import { contractAddresses, abi } from "../constants"
// dont export from moralis when using react
import { useMoralis } from "react-moralis"
import { useEffect, useState } from "react"
import { useNotification } from "web3uikit"
import { ethers } from "ethers"
import { EnumType } from "typescript"

type Props = {
}

interface PlayerState {
    stoneType: EnumType;
    matchId: number;
    inGame: boolean;
};

const Info = (props: Props) => {
    // State hooks
    // https://stackoverflow.com/questions/58252454/react-hooks-using-usestate-vs-just-variables
    const [boardState, setBoardState] = useState("0")
    const [whosTurn, setWhosTurn] = useState("0")
    const [myPlayerState, setMyPlayerState] = useState("0")
    const [myMatchId, setMyMatchId] = useState(0)

    const { chainId: chainIdHex, isWeb3EnableLoading, isWeb3Enabled } = useMoralis();
    const chainId = 4; // parseInt(chainIdHex!); // FIXME: typescript number to index
    // const goGameAddresss = chainId in contractAddresses ? contractAddresses[chainId] : null;
    const goGameAddress = contractAddresses[chainId][0];

    /* View Functions */

    const { runContractFunction: getBoardState } = useWeb3Contract({
        abi: abi,
        contractAddress: goGameAddress, // specify the networkId
        functionName: "BoardState",
        params: {
            matchId: myMatchId
        },
    })

    const { runContractFunction: getWhosTurn } = useWeb3Contract({
        abi: abi,
        contractAddress: goGameAddress,
        functionName: "WhosTurn",
        params: {
            matchId: myMatchId
        },
    })

    const { runContractFunction: getMyPlayerState } = useWeb3Contract({
        abi: abi,
        contractAddress: goGameAddress,
        functionName: "MyPlayerState",
        params: {},
    })

    async function updateUIValues() { // FIXME: useEffect
        const myPlayerStateRaw: unknown = await getMyPlayerState()
        const myPlayerStateObject: PlayerState = myPlayerStateRaw as PlayerState
        const matchId = myPlayerStateObject.matchId
        console.log(`MatchId: ${matchId}`)
        setMyMatchId(matchId)
        
        //await getWhosTurn()
        // console.log(`WhosTurnData: ${whosTurnData}`)

        const myPlayerStateFromCall = JSON.stringify(myPlayerStateObject)
        const boardStateFromCall = JSON.stringify(await getBoardState())
        const whosTurnFromCall = JSON.stringify(await getWhosTurn())

        console.log(boardStateFromCall)
        console.log(whosTurnFromCall)
        console.log(myPlayerStateFromCall)

        setBoardState(boardStateFromCall)
        setWhosTurn(whosTurnFromCall)
        setMyPlayerState(myPlayerStateFromCall)


    }

    useEffect(() => {
        if(isWeb3Enabled) {
            updateUIValues()
        }
    }, [isWeb3Enabled])

    return (
        // Cell
        <div className={ 'w-[300px] h-[30px] relative '}>
        <div>Board State: {boardState}</div>
        <div>Who's Turn: {whosTurn}</div>
        <div>Player State: {myPlayerState}</div>
        </div>
    );
};

export default Info;