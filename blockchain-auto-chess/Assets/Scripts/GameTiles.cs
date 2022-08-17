using System;
using System.Collections;
using System.Collections.Generic;
using Army;
using Ultility;
using UnityEngine;


public class GameTiles : Singleton<GameTiles>
{
    public struct Tile
    {
        public Vector3 position;
        public Soldier occupier;
    }

    [SerializeField] private GameObject _tilePrefab;
    public readonly Tile[,] data = new Tile[8, 8];

    public void PlaceSoldier(Soldier soldier, Vector2Int index)
    {
        //check index is not out of bound & occupied
        if (index.x < 0 || index.x > 7 || index.y < 0 || index.y > 7)
            throw new ArgumentException($"{index} is out of bound");
        if (data[index.x, index.y].occupier != null && data[index.x, index.y].occupier != soldier) throw new ArgumentException($"{index} is already occupied");
        //place soldier
        data[soldier.IndexPos.x, soldier.IndexPos.y].occupier = null;
        data[index.x, index.y].occupier = soldier;
        soldier.IndexPos = index;
    }

    public bool IsIndexOutOfBound(Vector2Int index)
    {
        return index.x < 0 || index.x > 7 || index.y < 0 || index.y > 7;
    }

    protected override void Awake()
    {
        InstantiateTiles();
    }

    private void InstantiateTiles()
    {
        // instantiate tiles
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                GameObject tile = Instantiate(_tilePrefab, new Vector3(i, 0, j), _tilePrefab.transform.rotation,
                    transform);
                data[i, j].position = tile.transform.position;
            }
        }
    }
}