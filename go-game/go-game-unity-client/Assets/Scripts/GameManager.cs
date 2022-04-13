using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public interface IGameManager
{
    void FindMatchNStartGame();
}

[RequireComponent(typeof(IGoManager))]
public class GameManager : MonoBehaviour, IGameManager
{
    private IGoManager _goManager;
    private IUIManager _uiManager;
    private ILocalPlayer _localPlayer;
    private IMatchMaker _matchMaker;
    private PreGoGameSetUp _preGoGameSetUp;

    private void Awake()
    {
        _goManager = GetComponent<IGoManager>();
        _uiManager = GetComponent<IUIManager>();
        _matchMaker = GetComponent<IMatchMaker>();
        _localPlayer = GetComponent<ILocalPlayer>();
        _preGoGameSetUp = GetComponent<PreGoGameSetUp>();
        _goManager.GoGameEndEvent += OnGoGameEnd;
    }

    private void Start()
    {
        _uiManager.ShowStartPage();
        _matchMaker.RegisterLocalPlayer(_localPlayer.Data);
    }

    public void FindMatchNStartGame()
    {
        StartCoroutine(nameof(FinMatchNStartGameWrapper));
    }

    private IEnumerator FinMatchNStartGameWrapper()
    {
        // Request match
        print("request match");
        Task.Run(_matchMaker.RequestMatch);
        // pre game set up
        yield return _preGoGameSetUp.DoCoroutine();
    }

    public void StartGame()
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