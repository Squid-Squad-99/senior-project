import { useMoralisQuery } from 'react-moralis'
import './App.css';
import Header from 'components/Header';
import Board from 'components/Board';

function App() {
  // const { data: playerState, isFetching: fetchingPlayerState}

  return (
    <div className="App">
      <Header />
      <hr />
      <Board />
    </div>
  );
}

export default App;
