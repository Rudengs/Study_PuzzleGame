﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move
}

public class Board : MonoBehaviour {

    private BackgroundTile[,] allTiles;
    private FindMatches findMatches;


    public int width;
    public int height;
    public int offSet;
    public Dot curDot;
    public GameState curState = GameState.move;
    public GameObject tilePrefab;
    public GameObject destroyEffect;
    public GameObject[] dots;
    public GameObject[,] allDots;
    // Use this for initialization
    void Start () 
    {
        allTiles = new BackgroundTile[width, height];
        allDots = new GameObject[width, height];
        findMatches = FindObjectOfType<FindMatches>();
        SetUp();
	}
	
    private void SetUp()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPos = new Vector2(i, j + offSet);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPos, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "( " + i + ", " + j + " )";

                int dotToUse = Random.Range(0, dots.Length);
                while(MathcesAt(i, j, dots[dotToUse]))
                    dotToUse = Random.Range(0, dots.Length);
                
                GameObject dot = Instantiate(dots[dotToUse], tempPos, Quaternion.identity);
                dot.GetComponent<Dot>().column = i;
                dot.GetComponent<Dot>().row = j;

                dot.transform.parent = this.transform;
                dot.name = "( " + i + ", " + j + " )";
                allDots[i, j] = dot;
            }
        }
    }

    private bool MathcesAt(int column, int row, GameObject piece)
    {
        if(1 < column && 1 < row)
        {
            if(allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row])
                return true;
            if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2])
                return true;
        }
        else if(column <= 1 || row <= 1)
        {
            if(1 < row)
            {
                if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2])
                    return true;
            }
            if(1 < column)
            {
                if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row])
                    return true;
            }
        }
        return false;
    }

    private bool ColumnOrRow()
    {
        int numberHorizontal = 0;
        int numberVertical = 0;

        Dot firstPiece = findMatches.curMatches[0].GetComponent<Dot>();
        if (firstPiece != null)
        {
            foreach (GameObject curPiece in findMatches.curMatches)
            {
                Dot dot = curPiece.GetComponent<Dot>();

                if (dot.row == firstPiece.row)
                    numberHorizontal++;
                if (dot.column == firstPiece.column)
                    numberVertical++;
            }
        }
        return (numberVertical == 5 || numberHorizontal == 5);
    }

    private void CheckToMakeBombs()
    {
        if (findMatches.curMatches.Count == 4 || findMatches.curMatches.Count == 7)
            findMatches.CheckBombs();
        if (findMatches.curMatches.Count == 5 || findMatches.curMatches.Count == 8)
        {
            if (ColumnOrRow())
            {
                if(curDot != null)
                {
                    if(curDot.isMatched)
                    {
                        curDot.isMatched = false;
                        if (!curDot.isColorBomb)
                        {
                            curDot.MakeColorBomb();
                        }
                    }
                    else
                    {
                        if(curDot.otherDot!= null)
                        {
                            Dot otherDot = curDot.otherDot.GetComponent<Dot>();
                            if(otherDot.isMatched)
                            {
                                otherDot.isMatched = false;
                                if (!otherDot.isColorBomb)
                                {
                                    otherDot.MakeColorBomb();
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (curDot != null)
                {
                    if (curDot.isMatched)
                    {
                        curDot.isMatched = false;
                        if (!curDot.isAdjacentbomb)
                        {
                            curDot.MakeAdjajcentBomb();
                        }
                    }
                    else
                    {
                        if (curDot.otherDot != null)
                        {
                            Dot otherDot = curDot.otherDot.GetComponent<Dot>();
                            if (otherDot.isMatched)
                            {
                                otherDot.isMatched = false;
                                if (!otherDot.isAdjacentbomb)
                                {
                                    otherDot.MakeAdjajcentBomb();
                                }
                            }
                        }
                    }
                }
            }
        }

    }

    private void DestroyMatchesAt(int column, int row)
    {
        if(allDots[column,row].GetComponent<Dot>().isMatched)
        {
            if (4 <= findMatches.curMatches.Count)
                CheckToMakeBombs();

            GameObject particle = Instantiate(destroyEffect, allDots[column, row].transform.position, Quaternion.identity);
            Destroy(particle, .5f);
            Destroy(allDots[column, row]);
            allDots[column, row] = null;
        }
    }

    public void DestroyMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                    DestroyMatchesAt(i, j);
            }
        }
        findMatches.curMatches.Clear();
        StartCoroutine(DecreaseRowCo());
    }

    private IEnumerator DecreaseRowCo()
    {
        int nullCount = 0;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null)
                    nullCount++;
                else if (0 < nullCount)
                {
                    allDots[i, j].GetComponent<Dot>().row -= nullCount;
                    allDots[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.6f);

        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allDots[i,j] == null)
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    int dotToUse = Random.Range(0, dots.Length);
                    GameObject newDot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    allDots[i, j] = newDot;
                    newDot.GetComponent<Dot>().column = i;
                    newDot.GetComponent<Dot>().row = j;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(allDots[i, j] != null)
                {
                    if(allDots[i, j].GetComponent<Dot>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo()
    {
        RefillBoard();
        yield return new WaitForSeconds(.5f);

        while(MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
        findMatches.curMatches.Clear();
        yield return new WaitForSeconds(.5f);
        curState = GameState.move;
    }

}
