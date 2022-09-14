using System;
using System.Collections;
using Army;
using UI;
using Ultility;
using UnityEngine;

public class LocalUser : Singleton<LocalUser>
{
    public int SelectedCardFrameIndex => GameUIController.Instance.CardGroupPanel.SelectedIndex;
    public Tile MouseHitTile { get; private set; }

    public Player LocalPlayer { get; private set; }

    private MouseHitProvider _mouseHitProvider;

    public void Init(Player localPlayer)
    {
        LocalPlayer = localPlayer;
    }

    protected override void Awake()
    {
        base.Awake();
        _mouseHitProvider = GetComponent<MouseHitProvider>();
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
        PlaceSoldier();
    }

    private void PlaceSoldier()
    {
        // use card
        if (SelectedCardFrameIndex != -1 && MouseHitTile != null)
        {
            LocalPlayer.UseCard(SelectedCardFrameIndex, MouseHitTile.Index);
        }
    }

    private IEnumerator MouseCardSelection()
    {
        CardGroupPanel cardGroupPanel = GameUIController.Instance.CardGroupPanel;
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