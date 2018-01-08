using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Box : MonoBehaviour {

    [HideInInspector]
    public int Index;


    [HideInInspector] public int LeftLine;
    [HideInInspector] public int RightLine;
    [HideInInspector] public int UpLine;
    [HideInInspector] public int BottomLine;

    public Color OccupiedColor;

    Color StartColor;

    // Use this for initialization
    void Start () {
        int Y_Index = Index / GameStats.Instance.BoxesInWidth;


        LeftLine = Index + Y_Index;
        RightLine = LeftLine + 1;
        BottomLine = (GameStats.Instance.BoxesInWidth + 1) * GameStats.Instance.BoxesInHight + Index;
        UpLine = BottomLine + GameStats.Instance.BoxesInWidth;

        transform.localScale = Vector3.one * GameStats.Instance.BoxWide / 5.12f;

        StartColor = GetComponent<SpriteRenderer>().color;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnMouseDown()
    {
        if (GameManager.Instance.CanTurn)
            Occupy();

    }

    public bool Occupy()
    {
        int Test = 0;

        if (!GameManager.Instance.MyLines[LeftLine].IsOccupied)
            Test = 1;

        if (!GameManager.Instance.MyLines[RightLine].IsOccupied)
            if (Test == 0)
                Test = 2;
            else
                return false;

        if (!GameManager.Instance.MyLines[UpLine].IsOccupied)
            if (Test == 0)
                Test = 3;
            else
                return false;

        if (!GameManager.Instance.MyLines[BottomLine].IsOccupied)
            if (Test == 0)
                Test = 4;
            else
                return false;


        if (Test == 0)
            return false;

        switch(Test)
        {

            case 1:
                {
                    GameManager.Instance.MyLines[LeftLine].OnMouseDown();
                    break;
                }
            case 2:
                {
                    GameManager.Instance.MyLines[RightLine].OnMouseDown();
                    break;
                }
            case 3:
                {
                    GameManager.Instance.MyLines[UpLine].OnMouseDown();
                    break;
                }
            case 4:
                {
                    GameManager.Instance.MyLines[BottomLine].OnMouseDown();
                    break;
                }
        }
        return true;

    }

    public void Refresh()
    {
        GetComponent<SpriteRenderer>().color = StartColor;
    }
    public bool CheckIsFill()
    {
        if (!GameManager.Instance.MyLines[LeftLine].IsOccupied)
            return false;

        if (!GameManager.Instance.MyLines[RightLine].IsOccupied)
            return false;

        if (!GameManager.Instance.MyLines[UpLine].IsOccupied)
            return false;

        if (!GameManager.Instance.MyLines[BottomLine].IsOccupied)
            return false;


        return true;
    }

    public bool isHasLine(int inIndex)
    {
        return (LeftLine == inIndex) || (RightLine == inIndex) || (UpLine == inIndex) || (BottomLine == inIndex);
    }
}
