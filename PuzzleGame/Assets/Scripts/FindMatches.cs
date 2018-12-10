using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                                if(!curMatches.Contains(leftDot))
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
}
