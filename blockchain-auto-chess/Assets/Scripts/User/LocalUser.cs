using System;
using System.Collections;
using Army;
using GameCore;
using TileMap;
using UI;
using UI.GameCore;
using Ultility;
using UnityEngine;

public class LocalUser : MonoBehaviour
{
    public int SelectedCardFrameIndex => _gameUIController.CardGroupPanel.SelectedIndex;
    public GamePlayer LocalGamePlayer { get; private set; }
    public Tile MouseHitTile { get; private set; }

    private MouseHitProvider _mouseHitProvider;
    [SerializeField] private GameUIController _gameUIController;


    protected void Awake()
    {
        _mouseHitProvider = GetComponent<MouseHitProvider>();
        LocalGamePlayer = GetComponent<GamePlayer>();
        _gameUIController.HookState(LocalGamePlayer);
    }

    private void Start()
    {
        StartCoroutine(FocusMouseHitTile());
        StartCoroutine(MouseCardSelection());
    }

    /// <summary>
    /// On mouse click
    /// </summary>
    private void OnFire()
    {
        // use card
        if (SelectedCardFrameIndex != -1 && MouseHitTile != null &&
            LocalGamePlayer.HaveCardIndex(SelectedCardFrameIndex))
        {
            LocalGamePlayer.UseCard(SelectedCardFrameIndex, MouseHitTile.Index);
        }
    }

    private IEnumerator MouseCardSelection()
    {
        CardGroupPanel cardGroupPanel = _gameUIController.CardGroupPanel;
        cardGroupPanel.CardTagEvent += (index) =>
        {
            if (cardGroupPanel.SelectedIndex == index)
            {
                cardGroupPanel.DeSelectAll();
            }
            else
            {
                cardGroupPanel.SelectCard(index);
            }
        };
        yield return null;
    }

    private IEnumerator FocusMouseHitTile()
    {
        while (true)
        {
            if (_mouseHitProvider.MouseHitObject != null)
            {
                var newTile = _mouseHitProvider.MouseHitObject.GetComponent<Tile>();
                if (newTile != null)
                {
                    if (newTile != MouseHitTile)
                    {
                        if (MouseHitTile != null) MouseHitTile.Focus(false);
                        MouseHitTile = newTile;
                        MouseHitTile.Focus(true);
                    }
                }
            }
            else
            {
                if (MouseHitTile != null) MouseHitTile.Focus(false);
                MouseHitTile = null;
            }

            yield return null;
        }
    }
}