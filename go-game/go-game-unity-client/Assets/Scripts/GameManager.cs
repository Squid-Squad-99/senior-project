using System;
using UnityEngine;

public interface IGameManager
{
    void StartGame();
}

[RequireComponent(typeof(IGoManager))]
public class GameManager : MonoBehaviour, IGameManager
{ 
    private IGoManager _goManager;
    private IUIManager _uiManager;

    private void Awake()
    {
        _goManager = GetComponent<IGoManager>();
        _uiManager = GetComponent<IUIManager>();
        _goManager.GoGameEndEvent += OnGoGameEnd;
    }

    private void Start()
    {
        _uiManager.ShowStartPage();
    }

    public void StartGame()
    {
        _goManager.EndGoGame();
        _uiManager.HideAllPage();
        _goManager.StartGoGame();
    }

    private void OnGoGameEnd(StoneType winPlayer)
    {
        _uiManager.ShowGoGameOverPage(winPlayer);
    }
}