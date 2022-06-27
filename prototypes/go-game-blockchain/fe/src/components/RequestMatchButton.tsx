import { useWeb3Contract } from "react-moralis"
import { useNotification } from "web3uikit"

import abi from "../constants/abi.json"

type Props = {
    goGameAddress: string;
}

const RequestMatchButton = ({goGameAddress}: Props) => {
    const dispatch = useNotification()

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

    const handleOnClick = async () => {
        await requestMatch({
            onSuccess: handleSuccess,
            onError: (error) => console.log(error),
        })
        console.log(`requestMatch completed!`);
    }

    const handleSuccess = async (tx: any) => {
        await tx.wait(1)
        dispatch({
            type: "success",
            message: "requestMatch()",
            title: "Tx Completed",
            position: "topR",
        })
    }

    return (
        <button className="bg-slate-300 hover:bg-slate-400 text-white font-bold py-1.5 px-3 rounded-2xl ml-auto" onClick={handleOnClick}>Request match</button>
    )
}

export default RequestMatchButton;