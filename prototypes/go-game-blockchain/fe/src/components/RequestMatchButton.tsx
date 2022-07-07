import useGoGameContract from "hooks/useGoGameContract";
import { useContext, useState } from "react";
import { useAppContext } from "context/AppContextProvider";
import { useNotification } from "web3uikit";

type Props = {};

const RequestMatchButton = ({}: Props) => {
  const dispatch = useNotification();
  const { runFunc: requestMatch } = useGoGameContract({
    functionName: "requestMatch",
  });
  const appState = useAppContext();

  const handleOnClick = async () => {
    // change metaGameState
    appState.setMetaGameState("SendingMatchRequest");
    // send request match
    await requestMatch({
      onSuccess: handleSuccess,
      onError: (error) => {
        console.log(error);
      },
    });
  };

  const handleSuccess = async (tx: any) => {
    // change metaGameState
    appState.setMetaGameState("FindingMatch");
    // send ui notification
    dispatch({
      type: "success",
      message: "Match Request Sended",
      title: "Tx Completed",
      position: "topR",
    });
  };

  return (
    <>
      <button
        className="bg-slate-400 hover:bg-slate-500 text-white font-bold py-1.5 px-3 rounded-2xl ml-auto"
        onClick={handleOnClick}
      >
        Request match
      </button>
      
    </>
  );
};

export default RequestMatchButton;
