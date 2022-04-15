using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private IGoManager _goManager;
    private IUIManager _uiManager;
    private IPlayer _localPlayer;
    private IMatchMaker _matchMaker;
    private IRelayer _relayer;
    private PreGoGameNetworkCommunication _preGoGameNetworkCommunication;

    private void Awake()
    {
        _goManager = GetComponent<IGoManager>();
        _uiManager = GetComponent<IUIManager>();
        _localPlayer = GetComponent<IPlayer>();
        _matchMaker = GetComponent<IMatchMaker>();
        _relayer = GetComponent<IRelayer>();
        _preGoGameNetworkCommunication = GetComponent<PreGoGameNetworkCommunication>();
        _goManager.GoGameEndEvent += OnGoGameEnd;
    }

    private void Start()
    {
        // init UI
        _uiManager.ShowStartPage();
        // prepare match making
        _matchMaker.RegisterPlayer(_localPlayer.Data);
    }

    public void FindMatchNStartGoGame()
    {
        // starting pre go game communication
        // 1. request match
        // 2. get ticket
        // 3. communicate with peer
        StartCoroutine(DoCoroutine());

        IEnumerator DoCoroutine()
        {
            yield return _preGoGameNetworkCommunication.DoCoroutine();
        }
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