using System;
using UnityEngine;

public interface IGameManager
{
    void FindMatchNStartGame();
}

[RequireComponent(typeof(IGoManager))]
public class GameManager : MonoBehaviour, IGameManager
{
    [Header("Dependencies")] private IGoManager _goManager;
    private IUIManager _uiManager;
    private ILocalPlayer _localPlayer;
    private IMatchMaker _matchMaker;

    private void Awake()
    {
        _goManager = GetComponent<IGoManager>();
        _uiManager = GetComponent<IUIManager>();
        _matchMaker = GetComponent<IMatchMaker>();
        _localPlayer = GetComponent<ILocalPlayer>();
        _goManager.GoGameEndEvent += OnGoGameEnd;
    }

    private async void Start()
    {
        _uiManager.ShowStartPage();
        await _matchMaker.RegisterLocalPlayer(_localPlayer.Data);
    }

    public async void FindMatchNStartGame()
    {
        print("Find match and start game");
        // Request match
        await _matchMaker.RequestMatch();

        // wait for ticket
        void OnGetTicket(Ticket ticket)
        {
            // get ticket and know we can use p2p relay and relay have set up
            StartGame();
            _matchMaker.GetTicketEvent -= OnGetTicket;
        }

        _matchMaker.GetTicketEvent += OnGetTicket;
    }

    private void StartGame()
    {
        // clean up
        _goManager.EndGoGame();
        _uiManager.HideAllPage();
        // start game
        _goManager.StartGoGame();
    }

    private void OnGoGameEnd(StoneType winPlayer)
    {
        _uiManager.ShowGoGameOverPage(winPlayer);
    }
}