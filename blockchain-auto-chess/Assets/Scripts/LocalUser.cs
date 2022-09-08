using System;
using System.Collections;
using Army;
using UI;
using Ultility;
using UnityEngine;

public class LocalUser : Singleton<LocalUser>
{
    public CardFrame SelectedCardFrame => GameUIController.Instance.CardGroupPanel.SelectedCardFrame;
    public Tile MouseHitTile { get; private set; }
    
    private MouseHitProvider _mouseHitProvider;

    protected override void Awake()
    {
        base.Awake();
        _mouseHitProvider = GetComponent<MouseHitProvider>();
    }

    private void Start()
    {
        StartCoroutine(FocusMouseHitTile());
    }

    private void OnFire()
    {
        // placing soldier
        if (SelectedCardFrame != null && MouseHitTile != null)
        {
            SoldierFactory.Instance.CreateSoldier(
                SelectedCardFrame.SoldierNameEnum,
                MouseHitTile.Index,
                TeamColorTypes.Blue
            );
        }
    }

    private IEnumerator FocusMouseHitTile()
    {
        
        Tile preHitTile = null;
        while (true)
        {
            if (_mouseHitProvider.MouseHitObject != null)
            {
                var newTile = _mouseHitProvider.MouseHitObject.GetComponent<Tile>();
                if (newTile != null)
                {
                    if (MouseHitTile == null) MouseHitTile = newTile;
                    else if (MouseHitTile != newTile)
                    {
                        preHitTile = MouseHitTile;
                        MouseHitTile = newTile;
                        if (preHitTile != null) preHitTile.Focus(false);
                        if (MouseHitTile != null) MouseHitTile.Focus(true);
                    }
                }
            }
            else
            {
                if (MouseHitTile != null) MouseHitTile.Focus(false);
                MouseHitTile = null;
            }

            yield return new WaitForFixedUpdate();
        }
    }
}