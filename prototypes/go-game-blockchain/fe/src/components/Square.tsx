import { StoneType } from "./GameView";

type Props = {
  row: number;
  col: number;
  val: StoneType;
  onClick: (row: number, col: number, val: StoneType) => void;
  hoverVal?: StoneType;
};

const Square = ({ row, col, val, onClick, hoverVal }: Props) => {
  const beforeStyle = `before:content-[''] before:w-[2px] before:bg-black before:absolute before:left-1/2 before:-translate-x-1/2 ${
    row === 0 ? "before:top-1/2" : "before:top-0"
  } ${row === 18 ? "before:h-1/2" : "before:h-full"} `;
  const afterStyle = `after:content-[''] after:h-[2px] after:bg-black after:absolute after:top-1/2 after:-translate-y-1/2 ${
    col === 0 ? "after:left-1/2" : "after:left-0"
  } ${col === 18 ? "after:w-1/2" : "after:w-full"} `;

  return (
    // Cell
    <div className="bg-[#ba8c63] w-[30px] h-[30px] relative z-0">
      <div className={beforeStyle + afterStyle} />
      {/* Piece */}
      <div
        className={`w-full h-full rounded-[50%] scale-50 z-[50] ${
          hoverVal
            ? hoverVal === "black"
              ? val !== "white"
                ? `hover:bg-neutral-900`
                : ``
              : val !== "black"
              ? `hover:bg-neutral-50`
              : ``
            : ""
        } ${val === "white" ? "bg-white" : ""} ${
          val === "black" ? "bg-black" : ""
        }`}
        onClick={() => onClick(row, col, val)}
      />
    </div>
  );
};

export default Square;
