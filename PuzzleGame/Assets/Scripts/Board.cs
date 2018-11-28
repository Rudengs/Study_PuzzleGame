﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {

    public int width;
    public int height;
    public GameObject tilePrefab;
    private BackgroundTile[,] allTiles;

	// Use this for initialization
	void Start () 
    {
        allTiles = new BackgroundTile[width, height];
        SetUp();
	}
	
    private void SetUp()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPos = new Vector2(i, j);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPos, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "( " + i + ", " + j + " )";
            }
        }
    }
}