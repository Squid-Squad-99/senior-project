using System;
using System.Collections;
using System.Collections.Generic;
using Ultility;
using UnityEngine;


public class GameTiles : Singleton<GameTiles>
{
    public struct Tile
    {
        public Vector3 position;
    }
    
    [SerializeField] private GameObject _tilePrefab;
    public readonly Tile[,] data = new Tile[8, 8];


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