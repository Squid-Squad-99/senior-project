
type Props = {
    myMatchId: string;
    myStoneType: string;
    whosTurn: string;
};

  
const BoardStatus = (props: Props) => {


    return (
        <div>
            {props.myMatchId === "0" 
            ? (<div className="flex flex-row justify-end">
                <div className="bg-red-500 rounded-full w-2 h-2 place-self-center"></div>
                <div className="leading-8 h-8 w-fit px-2 text-black text-center rounded-full uppercase">
                    Not matched
                </div>
            </div>)
            : (<div className="flex flex-row justify-between">
                <div className="leading-8 h-8 w-40 bg-amber-700 text-amber-50 text-center rounded-full font-bold uppercase">
                    {props.whosTurn === "black" ? "⚫ " : "⚪ "} 
                    {props.whosTurn === props.myStoneType ? `Your ` : `${props.whosTurn}'s `}
                    Turn 
                </div>
                <div className="flex flex-row">
                    <div className="bg-lime-500 rounded-full w-2 h-2 place-self-center"></div>
                    <div className="leading-8 h-8 w-fit px-2 text-black text-center rounded-full uppercase">
                        Matched ({props.myMatchId})
                    </div>
                </div>
            </div>)
            }
        </div>
    );
};

export default BoardStatus;