using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FindMatches : MonoBehaviour {

    private Board board;
    public List<GameObject> curMatches = new List<GameObject>();

	// Use this for initialization
	void Start () 
    {
        board = FindObjectOfType<Board>();
	}
	
    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(.2f);
        for(int i = 0; i < board.width; i++)
        {
            for(int j = 0; j < board.height; j++)
            {
                GameObject curDot = board.allDots[i, j];
                if(curDot != null)
                {
                    if(i > 0 && i < board.width -1)
                    {
                        GameObject leftDot = board.allDots[i - 1, j];
                        GameObject rightDot = board.allDots[i + 1, j];
                        if(leftDot != null && rightDot != null)
                        {
                            if(leftDot.tag == curDot.tag && rightDot.tag == curDot.tag)
                            {
                                if(curDot.GetComponent<Dot>().isRowBomb 
                                || leftDot.GetComponent<Dot>().isRowBomb 
                                || rightDot.GetComponent<Dot>().isRowBomb)
                                {
                                    curMatches.Union(GetRowPieces(j));
                                }

                                if (curDot.GetComponent<Dot>().isColumnBomb)
                                    curMatches.Union(GetColumnPieces(i));
                                if (leftDot.GetComponent<Dot>().isColumnBomb)
                                    curMatches.Union(GetColumnPieces(i-1));
                                if (rightDot.GetComponent<Dot>().isColumnBomb)
                                    curMatches.Union(GetColumnPieces(i+1));

                                if (!curMatches.Contains(leftDot))
                                    curMatches.Add(leftDot);
                                if (!curMatches.Contains(rightDot))
                                    curMatches.Add(rightDot);
                                if (!curMatches.Contains(curDot))
                                    curMatches.Add(curDot);

                                leftDot.GetComponent<Dot>().isMatched = true;
                                rightDot.GetComponent<Dot>().isMatched = true;
                                curDot.GetComponent<Dot>().isMatched = true;
                            }
                        }
                    }
                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject upDot = board.allDots[i, j + 1];
                        GameObject downDot = board.allDots[i, j - 1];
                        if (upDot != null && downDot != null)
                        {
                            if (upDot.tag == curDot.tag && downDot.tag == curDot.tag)
                            {
                                if (curDot.GetComponent<Dot>().isColumnBomb
                                || upDot.GetComponent<Dot>().isColumnBomb
                                || downDot.GetComponent<Dot>().isColumnBomb)
                                {
                                    curMatches.Union(GetColumnPieces(i));
                                }

                                if (curDot.GetComponent<Dot>().isRowBomb)
                                    curMatches.Union(GetRowPieces(j));
                                if (downDot.GetComponent<Dot>().isRowBomb)
                                    curMatches.Union(GetRowPieces(j-1));
                                if (upDot.GetComponent<Dot>().isRowBomb)
                                    curMatches.Union(GetRowPieces(j+1));

                                if (!curMatches.Contains(upDot))
                                    curMatches.Add(upDot);
                                if (!curMatches.Contains(downDot))
                                    curMatches.Add(downDot);
                                if (!curMatches.Contains(curDot))
                                    curMatches.Add(curDot);

                                upDot.GetComponent<Dot>().isMatched = true;
                                downDot.GetComponent<Dot>().isMatched = true;
                                curDot.GetComponent<Dot>().isMatched = true;
                            }
                        }
                    }
                }
            }
        }
    }

    List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> dots = new List<GameObject>();
        for(int i = 0; i < board.height; i++)
        {
            if(board.allDots[column, i] != null)
            {
                dots.Add(board.allDots[column, i]);
                board.allDots[column, i].GetComponent<Dot>().isMatched = true;
            }
        }
        return dots;
    }

    List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            if (board.allDots[i, row] != null)
            {
                dots.Add(board.allDots[i, row]);
                board.allDots[i, row].GetComponent<Dot>().isMatched = true;
            }
        }
        return dots;
    }

    public void CheckBombs()
    {
        // Did the player move something
        if(board.curDot != null)
        {
            //Is teh iece they move matched;
            if(board.curDot.isMatched)
            {
                // make it matched
                board.curDot.isMatched = false;
                // Descide what kind of bomb to make
                int typeOfBomb = Random.Range(0, 100);
                if(typeOfBomb < 50)
                {
                    // make a row bomb
                    board.curDot.MakeRowBomb();
                }
                else if(50 < typeOfBomb)
                {
                    // make a column bomb
                    board.curDot.MakeColumnBomb();
                }
            }
            else if(board.curDot.otherDot != null)
            {

            }
        }
    }
}
