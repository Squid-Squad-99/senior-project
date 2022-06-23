import { useMoralis } from "react-moralis"

type Props = {
    text?: string;
}
  
const ConnectButton = ({text}: Props) => {
  const { enableWeb3, isWeb3EnableLoading, account } = useMoralis()

  const handleOnClick = async () => {
    // await walletModal.connect()
    await enableWeb3()
    // depends on what button they picked
    if (typeof window !== "undefined") {
        window.localStorage.setItem("connected", "injected")
        // window.localStorage.setItem("connected", "walletconnect")
    }
    console.log(`Connected address: ${account}`);
  }

  return (
    // Cell
    <button 
      className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded ml-auto"
      onClick={handleOnClick}
      disabled={isWeb3EnableLoading}
    >
      {text}
    </button>
  );
};

export default ConnectButton;