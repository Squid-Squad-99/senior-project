using System;
using Ultility;

public class GameState : Singleton<GameState>
{
    public int Round
    {
        get => _round;
        set
        {
            _round = value;
            GameStateChangeEvent?.Invoke();
        }
    }

    public int RedWinCnt
    {
        get => _redWindCnt;
        set
        {
            _redWindCnt = value;
            GameStateChangeEvent?.Invoke();
        }
    }

    public int BlueWinCnt
    {
        get => _blueWinCnt;
        set
        {
            _blueWinCnt = value;
            GameStateChangeEvent?.Invoke();
        }
    }

    private int _round = 1;
    private int _redWindCnt = 0;
    private int _blueWinCnt = 0;
    public event Action GameStateChangeEvent;
}

public class Spec_GameState
{
    public string P1ID = "player1";
    public string P2ID = "player2";
    public int Round = 1;
    public int P1WinCnt = 0;
    public int P2WinCnt = 0;
    public SoldierType[] P1Cards = new SoldierType[5];
    public SoldierType[] P2Cards = new SoldierType[5];
    public TileState[,] Board = new TileState[8, 8];
    public StageType Stage = StageType.WaitingP1P2;

    public enum SoldierType
    {
        None,
        Base,
    }

    public class TileState
    {
        public SoldierType Soldier;
        public bool IsP1Soldier;
    }

    public enum StageType
    {
        WaitingP1P2,
        WaitingP1,
        WaitingP2
    }
    
}

public class Spec_UseCardAction
{
    public int CardIndex;
    public int X, Y;
}