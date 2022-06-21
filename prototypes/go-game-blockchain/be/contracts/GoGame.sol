//SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

import "hardhat/console.sol";
import "./Convert.sol";

error GoGame__PlayerDontHaveMatch();
error GoGame__GameIsOver();
error GoGame__IsNotYourTurn();
error GoGame__InvalidPlacing();

contract GoGame {
  enum StoneType {
    None,
    Black,
    White
  }

  struct GameState {
    StoneType[19 * 19] boardState; // current board state
    StoneType turn; // whos turn is it now
    bool isOver;
  }

  struct PlayerState {
    StoneType stoneType;
    uint256 matchId;
    bool inGame;
  }

  event FindMatch(uint256 indexed matchId, address player1, address player2);
  event GameOver(uint256 indexed matchId, address winner);

  address[] matchQueue;
  mapping(uint256 => GameState) matchIdToGS; // index by match id
  mapping(address => PlayerState) addressToPS; // index by address
  uint32 matchCnt;

  /* public & external function  */

  function requestMatch() public {
    // add to match queue
    matchQueue.push(msg.sender);

    // check for match
    if (matchQueue.length == 2) {
      // get player
      address p1 = matchQueue[0];
      address p2 = matchQueue[1];
      // create new game
      uint256 matchId = createNewGame(p1, p2);
      // emit event
      emit FindMatch(matchId, p1, p2);
      // clear match queue
      delete matchQueue;
    }
  }

  // player place stone
  function PlaceStone(
    uint8 x,
    uint8 y,
    bool checkWin
  ) public {
    // find out player match
    PlayerState memory ps = addressToPS[msg.sender];
    if (ps.matchId == 0) {
      revert GoGame__PlayerDontHaveMatch();
    }
    // check is game is not over and is player turn
    GameState memory gs = matchIdToGS[ps.matchId];
    if (gs.isOver) {
      revert GoGame__GameIsOver();
    }
    if (gs.turn != ps.stoneType) {
      revert GoGame__IsNotYourTurn();
    }
    // check place index is valid
    if (
      (x < 0 || x >= 19) ||
      (y < 0 || y >= 19) ||
      gs.boardState[x + y * 19] != StoneType.None
    ) {
      revert GoGame__InvalidPlacing();
    }

    // place stone
    matchIdToGS[ps.matchId].boardState[x + y * 19] = ps.stoneType;
    matchIdToGS[ps.matchId].turn = ps.stoneType == StoneType.White
      ? StoneType.Black
      : StoneType.White;

    if (checkWin) {
      // check win condition
      // 1. up -> right -> down -> left
      uint8[8] memory cnt;
      int8[2][4][8] memory vecs = [
        [
          [int8(0), int8(1)],
          [int8(0), int8(2)],
          [int8(0), int8(3)],
          [int8(0), int8(4)]
        ],
        [
          [int8(1), int8(1)],
          [int8(2), int8(2)],
          [int8(3), int8(3)],
          [int8(4), int8(4)]
        ],
        [
          [int8(1), int8(0)],
          [int8(2), int8(0)],
          [int8(3), int8(0)],
          [int8(4), int8(0)]
        ],
        [
          [int8(1), int8(-1)],
          [int8(2), int8(-2)],
          [int8(3), int8(-3)],
          [int8(4), int8(-4)]
        ],
        [
          [int8(0), int8(-1)],
          [int8(0), int8(-2)],
          [int8(0), int8(-3)],
          [int8(0), int8(-4)]
        ],
        [
          [int8(-1), int8(-1)],
          [int8(-2), int8(-2)],
          [int8(-3), int8(-3)],
          [int8(-4), int8(-4)]
        ],
        [
          [int8(-1), int8(0)],
          [int8(-2), int8(0)],
          [int8(-3), int8(0)],
          [int8(-4), int8(0)]
        ],
        [
          [int8(-1), int8(1)],
          [int8(-2), int8(2)],
          [int8(-3), int8(3)],
          [int8(-4), int8(4)]
        ]
      ];

      for (uint8 i = 0; i < 8; i++) {
        for (uint8 j = 0; j < 4; j++) {
          // checking index
          int8 nx = int8(x) + vecs[i][j][0];
          int8 ny = int8(y) + vecs[i][j][1];
          // check out of bound
          if (nx < 0 || nx >= 19 || ny < 0 || ny >= 19) break;
          // if line is continuous
          if(gs.boardState[uint16(uint8(nx)) + 19 * uint16(uint8(ny))] == ps.stoneType){
              // cnt add
              cnt[i]++;
          }
          else{
              // this line is done
              break;
          }
        }
      }

        // check have 5 in a line
      for(uint8 i = 0; i < 4; i++){
          if(cnt[i] + cnt[i+4] >= 4){
              // win
              matchIdToGS[ps.matchId].isOver = true;
              emit GameOver(ps.matchId, msg.sender);
              break;
          }
      }
    }
  }

  /* private & internal function */

  function createNewGame(address p1, address p2)
    private
    returns (uint256 matchId)
  {
    // initial game state
    StoneType[361] memory boardState;
    GameState memory gs = GameState(boardState, StoneType.White, false);
    // add to map
    matchIdToGS[++matchCnt] = gs;
    // decide who is white/black
    bool p1IsWhite = decideStoneType(p1, p2);
    // init player state
    StoneType p1Stone = p1IsWhite ? StoneType.White : StoneType.Black;
    StoneType p2Stone = p1IsWhite ? StoneType.Black : StoneType.White;
    // add to map
    addressToPS[p1] = PlayerState(p1Stone, matchCnt, true);
    addressToPS[p2] = PlayerState(p2Stone, matchCnt, true);

    return matchCnt;
  }

  function decideStoneType(address p1, address p2)
    private
    view
    returns (bool p1IsWhite)
  {
    bytes32 randomHash = sha256(
      abi.encodePacked(
        Convert.addressToBytes32(p1) ^
          Convert.addressToBytes32(p2) ^
          Convert.uint256ToBytes32(block.timestamp)
      )
    );

    if (uint256(randomHash) % 2 == 0) {
      // p1 is white
      return true;
    } else {
      // p2 is white
      return false;
    }
  }

  /* pure & view function */
  function BoardState(uint256 matchId)
    public
    view
    returns (StoneType[361] memory)
  {
    return matchIdToGS[matchId].boardState;
  }

  function WhosTurn(uint256 matchId) public view returns (StoneType) {
    return matchIdToGS[matchId].turn;
  }

  function MyPlayerState() public view returns (PlayerState memory) {
    return addressToPS[msg.sender];
  }
}
