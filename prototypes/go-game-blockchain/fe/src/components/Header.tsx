import { ConnectButton } from "web3uikit"
import  RequestMatchButton from "./RequestMatchButton"

type Props = {
  goGameAddress: string;
  myMatchId: string;
}

const Header = (props: Props) => {

  return (
    <div className="py-4 px-4 flex flex-row justify-between items-center">
        <div className="self-center font-bold text-xl">Blockchain Game</div>
        <div className="flex flex-row space-x-2">         
            <RequestMatchButton goGameAddress={props.goGameAddress}/>
            <ConnectButton moralisAuth={false}/>
        </div>
    </div>
  );
};

export default Header;