using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IGoManager
{
    event Action<StoneType> GoGameEndEvent;
    void StartGoGame();
    void EndGoGame();
}

/// <summary>
/// Integrate behavior of go logic, board, input
/// </summary>
[RequireComponent(typeof(IMouseInGameDataProvider), typeof(PlayerInput))]
public class GoManager : MonoBehaviour, IGoManager
{
    private IMouseInGameDataProvider _mouseInGameDataProvider;
    private IGoLogic _goLogic;
    private IBoard _board;
    private StoneType _currentPlayer;
    private event Action PlaceStoneEvent;
    private event Action SelectBoardEvent;

    public event Action<StoneType> GoGameEndEvent;

    private void Awake()
    {
        _mouseInGameDataProvider = GetComponent<IMouseInGameDataProvider>();
        _goLogic = GetComponent<IGoLogic>();
        _board = GameObject.Find("GoBoard").GetComponent<IBoard>();
    }

    public void StartGoGame()
    {
        // listen for win event
        void OnWin(StoneType winPlayer)
        {
            print($"{winPlayer} win!!");
            GoGameEndEvent?.Invoke(winPlayer);
            _goLogic.WinEvent -= OnWin;
            PlaceStoneEvent -= OnPlaceStone;
            SelectBoardEvent -= OnSelectBoard;
        }

        void OnPlaceStone()
        {
            // switch current player
            _currentPlayer = (_currentPlayer == StoneType.Black) ? StoneType.White : StoneType.Black;
        }

        void OnSelectBoard()
        {
            int2 placeItersectionIndex = _board.NearestIntersectionIndex(_mouseInGameDataProvider.MouseWOrldPosition);
            if (_goLogic.IsIntersectionOccupied(placeItersectionIndex))
            {
                print("this intersection is occupied");
            }
            else
            {
                // place a stone
                PlaceStone(_currentPlayer, placeItersectionIndex);
            }
        }

        _goLogic.WinEvent += OnWin;
        PlaceStoneEvent += OnPlaceStone;
        SelectBoardEvent += OnSelectBoard;

        // white go first
        _currentPlayer = StoneType.White;
    }

    private void PlaceStone(StoneType stoneType, int2 xy)
    {
        _board.PlaceStone(stoneType, xy);
        _goLogic.PlaceStone(stoneType, xy);
        PlaceStoneEvent?.Invoke();
    }

    public void EndGoGame()
    {
        _board.Clear();
        _goLogic.ClearBoard();
    }

    private void OnSelect()
    {
        // check hit anything
        if (_mouseInGameDataProvider.MouseHit == null) return;

        // if mouse hit board
        IBoard board = _mouseInGameDataProvider.MouseHit.transform.GetComponent<IBoard>();
        if (board != null)
        {
            SelectBoardEvent?.Invoke();
        }
    }
}