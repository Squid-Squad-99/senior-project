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