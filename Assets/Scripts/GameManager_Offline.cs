using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager_Offline : GameManager {

    // calculating width of the box based on biggest number of those boxes, generate grid (boxes/lines/dots), clear general score
    void Start () {

        GameStats.Instance.FirstWins = 0;
        GameStats.Instance.SecondWins = 0;

        int MainLength = GameStats.Instance.BoxesInWidth > GameStats.Instance.BoxesInHight ? GameStats.Instance.BoxesInWidth : GameStats.Instance.BoxesInHight;

        GameStats.Instance.BoxWide = 6f / MainLength;

        if (MyCamera != null)
            MyCamera.transform.position = new Vector3( GameStats.Instance.BoxesInWidth * GameStats.Instance.BoxWide / 2f,
                GameStats.Instance.BoxesInHight * GameStats.Instance.BoxWide / 2, -10);

        GridGenerator.GenerateGrid(out MyDots, out MyLines, out MyBoxes, 6f / MainLength, GameStats.Instance.BoxesInWidth, GameStats.Instance.BoxesInHight, MyBox, MyLine, MyDot);

       

    }
	
    public override void MakeTurnByLine(int inIndex)
    {
        int BoxCount = MyBoxes.Count;


        bool AllBoxesFill = true;
        int BoxesFound = 0;

        for (int i = 0; i < BoxCount; ++i)
        {
            if (AllBoxesFill)
                if (!MyBoxes[i].CheckIsFill())
                    AllBoxesFill = false;               // checking on win game


            // how many boxes filled after filling our line
            if (MyBoxes[i].isHasLine(inIndex))          
                if (MyBoxes[i].CheckIsFill())
                {
                    MyBoxes[i].GetComponent<SpriteRenderer>().color = IsFirstTurn ? GameStats.Instance.FirstColor : GameStats.Instance.SecondColor;
                    ++BoxesFound;
                }

        }


        if (IsFirstTurn)
            FirstScore += BoxesFound;
        else
            SecondScore += BoxesFound;

        if (AllBoxesFill)
        {
            if (FirstScore > SecondScore)
                GameStats.Instance.FirstWins++;
            if (SecondScore > FirstScore)
                GameStats.Instance.SecondWins++;
            GameOver();
            return;
        }

        IsFirstTurn = !IsFirstTurn;
    }

    // refresh all boxes and lines, and local score
    public override void NewGame()
    {
        FirstScore = 0;
        SecondScore = 0;

        GameOverScreen.SetActive(false);
        MyHUD.SetActive(true);

        foreach (S_Box b in MyBoxes)
            b.Refresh();

        foreach (S_Line l in MyLines)
            l.HardOccupy(false);
    }

    protected override void GameOver()
    {
        GameOverScreen.SetActive(true);
        MyHUD.SetActive(false);
    }

    public void GoMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
