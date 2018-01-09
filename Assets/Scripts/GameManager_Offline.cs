using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager_Offline : GameManager {

	// Use this for initialization
	void Start () {

        int MainLength = GameStats.Instance.BoxesInWidth > GameStats.Instance.BoxesInHight ? GameStats.Instance.BoxesInWidth : GameStats.Instance.BoxesInHight;

        GameStats.Instance.BoxWide = 6f / MainLength;

        if (MyCamera != null)
            MyCamera.transform.position = new Vector3( GameStats.Instance.BoxesInWidth * GameStats.Instance.BoxWide / 2f,
                GameStats.Instance.BoxesInHight * GameStats.Instance.BoxWide / 2, -10);

        GridGenerator.GenerateGrid(out MyDots, out MyLines, out MyBoxes, 6f / MainLength, GameStats.Instance.BoxesInWidth, GameStats.Instance.BoxesInHight, MyBox, MyLine, MyDot);

       

    }
	
	// Update is called once per frame
	void Update () {
		
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
                    AllBoxesFill = false;

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
            GameOver();
            return;
        }

        IsFirstTurn = !IsFirstTurn;
    }

    public override void NewGame()
    {
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
