import { ConnectButton } from "web3uikit"

const Header = () => {
  return (
    <div className="py-4 px-4 flex flex-row justify-between items-center">
        <div className="self-center font-bold text-xl">Blockchain Game</div>
        <div className="flex flex-row">
            <button>Request Match</button>
            <ConnectButton moralisAuth={false} />
        </div>
    </div>
  );
};

export default Header;