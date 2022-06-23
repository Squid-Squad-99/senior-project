type Props = {
  row?: number;
  col?: number;
  val?: string;
  onClick: Function;
};

const Square = (props: Props) => {
  const beforeStyle = `before:content-[''] before:w-[2px] before:bg-black before:absolute before:left-1/2 before:-translate-x-1/2 ${
    props.row === 0 ? "before:top-1/2" : "before:top-0"
  } ${props.row === 18 ? "before:h-1/2" : "before:h-full"} `;
  const afterStyle = `after:content-[''] after:h-[2px] after:bg-black after:absolute after:top-1/2 after:-translate-y-1/2 ${
    props.col === 0 ? "after:left-1/2" : "after:left-0"
  } ${props.col === 18 ? "after:w-1/2" : "after:w-full"} `;
  const handleSquareClick = () => {
    props.onClick(props.row, props.col, props.val);
  };
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
