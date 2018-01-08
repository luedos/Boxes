using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager_Offline : GameManager {

	// Use this for initialization
	void Start () {
        GridGenerator.GenerateGrid(out MyDots, out MyLines, out MyBoxes, GameStats.Instance.BoxWide, GameStats.Instance.BoxesInWidth, GameStats.Instance.BoxesInHight, MyBox, MyLine, MyDot);
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

    protected override void NewGame()
    {
        foreach (S_Box b in MyBoxes)
            b.Refresh();

        foreach (S_Line l in MyLines)
            l.HardOccupy(false);
    }

    protected override void GameOver()
    {
        NewGame();
        base.GameOver();
    }
}
