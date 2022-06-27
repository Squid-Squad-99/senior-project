import { useWeb3Contract } from "react-moralis"
import { useNotification } from "web3uikit"

import abi from "../constants/abi.json"

type Props = {
    row?: number;
    col?: number;
    val?: string;
    onClick: Function;
    goGameAddress: string;
  };

  
  const Square = (props: Props) => {
    const beforeStyle = `before:content-[''] before:w-[2px] before:bg-black before:absolute before:left-1/2 before:-translate-x-1/2 ${
      props.row === 0 ? "before:top-1/2" : "before:top-0"
    } ${props.row === 18 ? "before:h-1/2" : "before:h-full"} `;
    const afterStyle = `after:content-[''] after:h-[2px] after:bg-black after:absolute after:top-1/2 after:-translate-y-1/2 ${
      props.col === 0 ? "after:left-1/2" : "after:left-0"
    } ${props.col === 18 ? "after:w-1/2" : "after:w-full"} `;

    const dispatch = useNotification()

    const {
      runContractFunction: placeStone,
      data: enterTxResponse,
      isLoading,
      isFetching
    } = useWeb3Contract({
        abi: abi,
        contractAddress: props.goGameAddress,
        functionName: 'PlaceStone',
        params: {
          x: props.col,
          y: props.row,
          checkWin: true
        }
      }   
    );

    const handleSquareClick = async () => {
      props.onClick(props.row, props.col, props.val);
      await placeStone({
        onSuccess: handleSuccess,
        onError: (error) => {console.log(error)},
      });
    };

    const handleSuccess = async (tx: any) => {
      await tx.wait(1)
      dispatch({
            type: "success",
            message: "placeStone()",
            title: "Tx Completed",
            position: "topR",
      })
      console.log("stone placed!")
      console.log(props.row, props.col, props.val)
    }

    return (
      // Cell
      <div
        className={
          "bg-[#ba8c63] w-[30px] h-[30px] relative " + beforeStyle + afterStyle
        }
      >
        {/* Piece */}
        <div
          className={`w-full h-full rounded-[50%] absolute scale-50 z-50 ${
            props.val === "white" ? "bg-white" : ""
          } ${props.val === "black" ? "bg-black" : ""}`}
          onClick={handleSquareClick}
        />
      </div>
    );
  };
  
  export default Square;