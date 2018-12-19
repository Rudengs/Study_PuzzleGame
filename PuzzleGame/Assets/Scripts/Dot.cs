
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour {

    [Header("Board Variables")]
    private Board board;
    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 tempPosition;
    private FindMatches findMatches;

    public int column;
    public int row;
    public int previousColumn;
    public int previousRow;
    public bool isMatched = false;
    public GameObject otherDot;

    [Header("Swipe Stuff")]
    public float swipeAngle = 0;
    public float swipeResist = 1f;

    [Header("Powerup Stuff")]
    public bool isRowBomb;
    public bool isColumnBomb;
    public bool isColorBomb;
    public bool isAdjacentbomb;
    public GameObject rowArrow;
    public GameObject columnArrow;
    public GameObject colorBomb;
    public GameObject adjacentBomb;

    // Use this for initialization
    void Start () {
        isRowBomb = false;
        isColumnBomb = false;
        isColorBomb = false;
        isAdjacentbomb = false;

        board = FindObjectOfType<Board>();
        findMatches = FindObjectOfType<FindMatches>();
    }

    // This is for testing and Debug only.
    private void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(1))
        {
            isAdjacentbomb = true;
            GameObject arrow = Instantiate(adjacentBomb, transform.position, Quaternion.identity);
            arrow.transform.parent = this.transform;
        }
    }

    // Update is called once per frame
    void Update () 
    {
        if (Mathf.Abs(column - transform.position.x) > .1)
        {
            // Move Towards the target
            tempPosition = new Vector2(column, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if (board.allDots[column, row] != this.gameObject)
                board.allDots[column, row] = this.gameObject;
            findMatches.FindAllMatches();
        }
        else
        {
            //Directly set the position
            tempPosition = new Vector2(column, transform.position.y);
            transform.position = tempPosition;
            board.allDots[column, row] = this.gameObject;
        }

        if (Mathf.Abs(row - transform.position.y) > .1)
        {
            // Move Towards the target
            tempPosition = new Vector2(transform.position.x, row);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if (board.allDots[column, row] != this.gameObject)
                board.allDots[column, row] = this.gameObject;
            findMatches.FindAllMatches();
        }
        else
        {
            //Directly set the position
            tempPosition = new Vector2(transform.position.x, row);
            transform.position = tempPosition;
            board.allDots[column, row] = this.gameObject;
        }
    }

    private void OnMouseDown()
    {
        if(board.curState == GameState.move)
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        if (board.curState == GameState.move)
        {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }

    public IEnumerator CheckMoveCo()
    {
        if(isColorBomb)
        {
            findMatches.MatchPiecesOfColor(otherDot.tag);
            isMatched = true;
        }
        else if(otherDot.GetComponent<Dot>().isColorBomb)
        {
            findMatches.MatchPiecesOfColor(this.gameObject.tag);
            otherDot.GetComponent<Dot>().isMatched = true;
        }
        yield return new WaitForSeconds(.5f);
        if (otherDot != null)
        {
            if (!isMatched && !otherDot.GetComponent<Dot>().isMatched)
            {
                otherDot.GetComponent<Dot>().row = row;
                otherDot.GetComponent<Dot>().column = column;
                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(.5f);
                board.curDot = null;
                board.curState = GameState.move;
            }
            else
            {
                board.DestroyMatches();
            }
            //otherDot = null;
        }
    }

    void CalculateAngle()
    {
        if (Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist ||
            Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            board.curState = GameState.wait;
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            MovePieces();
            board.curDot = this;
        }
        else
        {
            board.curState = GameState.move;
        }
    }

    void MovePiecesActual(Vector2 direction)
    {
        previousColumn = column;
        previousRow = row;

        otherDot = board.allDots[column + (int)direction.x, row + (int)direction.y];
        otherDot.GetComponent<Dot>().column += -1 * (int)direction.x;
        otherDot.GetComponent<Dot>().row += -1 * (int)direction.y;
        column += (int)direction.x;
        row += (int)direction.y;

        StartCoroutine(CheckMoveCo());
    }

    void MovePieces()
    {

        if (-45 < swipeAngle && swipeAngle <= 45 && column < board.width - 1)
        {
            // Right Swipe
            MovePiecesActual(Vector2.right);

        }
        else if (45 < swipeAngle && swipeAngle <= 135 && row < board.height - 1)
        {
            // Up Swipe
            MovePiecesActual(Vector2.up);
        }
        else if (135 < swipeAngle && swipeAngle <= -135 && 0 < column)
        {
            // Left Swipe
            MovePiecesActual(Vector2.left);
        }
        else if (-135 < swipeAngle && swipeAngle <= -45 && 0 < row)
        {
            // Down Swipe
            MovePiecesActual(Vector2.down);
        }
        board.curState = GameState.move;
    }

    void FindMatches()
    {
        if(0 < column && column < board.width - 1)
        {
            GameObject leftDot1 = board.allDots[column - 1, row];
            GameObject rightDot1 = board.allDots[column + 1, row];

            if (leftDot1 == null || rightDot1 == null)
                return;

            if (leftDot1.tag == this.gameObject.tag && rightDot1.tag == this.gameObject.tag)
            {
                leftDot1.GetComponent<Dot>().isMatched = true;
                rightDot1.GetComponent<Dot>().isMatched = true;
                isMatched = true;
            }
        }
        if (0 < row && row < board.height - 1)
        {
            GameObject downDot1 = board.allDots[column, row-1];
            GameObject upDot1 = board.allDots[column, row+1];

            if (downDot1 == null || upDot1 == null)
                return;

            if (downDot1.tag == this.gameObject.tag && upDot1.tag == this.gameObject.tag)
            {
                downDot1.GetComponent<Dot>().isMatched = true;
                upDot1.GetComponent<Dot>().isMatched = true;
                isMatched = true;
            }
        }
    }

    public void MakeRowBomb()
    {
        isRowBomb = true;
        GameObject arrow = Instantiate(rowArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = this.transform;
    }

    public void MakeColumnBomb()
    {
        isColumnBomb = true;
        GameObject arrow = Instantiate(columnArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = this.transform;
    }

    public void MakeColorBomb()
    {
        isColorBomb = true;
        GameObject color = Instantiate(colorBomb, transform.position, Quaternion.identity);
        color.transform.parent = this.transform;
    }

    public void MakeAdjajcentBomb()
    {
        isAdjacentbomb = true;
        GameObject marker = Instantiate(adjacentBomb, transform.position, Quaternion.identity);
        marker.transform.parent = this.transform;
    }
}
