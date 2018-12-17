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

    private List<GameObject> IsRowBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> curDots = new List<GameObject>();

        if (dot1.isRowBomb)
            curMatches.Union(GetRowPieces(dot1.row));
        if (dot2.isRowBomb)
            curMatches.Union(GetRowPieces(dot2.row));
        if (dot3.isRowBomb)
            curMatches.Union(GetRowPieces(dot3.row));

        return curDots;
    }

    private List<GameObject> IsColumnBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> curDots = new List<GameObject>();

        if (dot1.isColumnBomb)
            curMatches.Union(GetColumnPieces(dot1.column));
        if (dot2.isColumnBomb)
            curMatches.Union(GetColumnPieces(dot2.column));
        if (dot3.isColumnBomb)
            curMatches.Union(GetColumnPieces(dot3.column));

        return curDots;
    }

    private void AddToListAndMatch(GameObject dot)
    {
        if (!curMatches.Contains(dot))
            curMatches.Add(dot);

        dot.GetComponent<Dot>().isMatched = true;
    }

    private void GetNearbyPieces(GameObject dot1, GameObject dot2, GameObject dot3)
    {
        AddToListAndMatch(dot1);
        AddToListAndMatch(dot2);
        AddToListAndMatch(dot3);
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
                    Dot curDotDot = curDot.GetComponent<Dot>();
                    if (i > 0 && i < board.width -1)
                    {
                        GameObject leftDot = board.allDots[i - 1, j];
                        GameObject rightDot = board.allDots[i + 1, j];

                        if (leftDot != null && rightDot != null)
                        {
                            Dot leftDotDot = leftDot.GetComponent<Dot>();
                            Dot rightDotDot = rightDot.GetComponent<Dot>();

                            if (leftDot.tag == curDot.tag && rightDot.tag == curDot.tag)
                            {
                                curMatches.Union(IsRowBomb(curDotDot, leftDotDot, rightDotDot));
                                curMatches.Union(IsColumnBomb(curDotDot, leftDotDot, rightDotDot));
                                GetNearbyPieces(curDot, leftDot, rightDot);
                            }
                        }
                    }
                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject upDot = board.allDots[i, j + 1];
                        GameObject downDot = board.allDots[i, j - 1];

                        if (upDot != null && downDot != null)
                        {
                            Dot upDotDot = upDot.GetComponent<Dot>();
                            Dot downDotDot = downDot.GetComponent<Dot>();

                            if (upDot.tag == curDot.tag && downDot.tag == curDot.tag)
                            {
                                curMatches.Union(IsColumnBomb(curDotDot, upDotDot, downDotDot));
                                curMatches.Union(IsRowBomb(curDotDot, upDotDot, downDotDot));
                                GetNearbyPieces(curDot, upDot, downDot);
                            }
                        }
                    }
                }
            }
        }
    }

    public void MatchPiecesOfColor(string color)
    {
        for(int i = 0; i < board.width; i++)
        {
            for(int j = 0; j < board.height; j++)
            {
                if(board.allDots[i, j] != null)
                {
                    if(board.allDots[i, j].tag == color)
                    {
                        board.allDots[i, j].GetComponent<Dot>().isMatched = true;
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

                if((-45 < board.curDot.swipeAngle && board.curDot.swipeAngle <= 45)
                || (board.curDot.swipeAngle < -135 || 135 <= board.curDot.swipeAngle))
                {
                    board.curDot.MakeRowBomb();
                }
                else
                {
                    board.curDot.MakeColumnBomb();
                }
            }
            else if(board.curDot.otherDot != null)
            {
                Dot otherDot = board.curDot.otherDot.GetComponent<Dot>();
                if(otherDot.isMatched)
                {
                    otherDot.isMatched = false;

                    if ((-45 < board.curDot.swipeAngle && board.curDot.swipeAngle <= 45)
                    || (board.curDot.swipeAngle < -135 || 135 <= board.curDot.swipeAngle))
                    {
                        otherDot.MakeRowBomb();
                    }
                    else
                    {
                        otherDot.MakeColumnBomb();
                    }
                }
            }
        }
    }
}
