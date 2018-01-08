using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    static GameManager instance = null;
    
    [HideInInspector] public List<S_Box> MyBoxes = new List<S_Box>();
    [HideInInspector] public List<S_Line> MyLines = new List<S_Line>();
    [HideInInspector] public List<S_Dot> MyDots = new List<S_Dot>();

    public GameObject MyBox;
    public GameObject MyLine;
    public GameObject MyDot;

    [HideInInspector] public bool IsFirstTurn = true;
    [HideInInspector] public int FirstScore = 0;
    [HideInInspector] public int SecondScore = 0;

    [HideInInspector] public bool CanTurn = true;

    public static GameManager Instance { get { return instance; } }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if(instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    // Use this for initialization
    void Start () {

        //GridGenerator.GenerateGrid(out MyDots, out MyLines, out MyBoxes, GameStats.Instance.BoxWide, GameStats.Instance.BoxesInWidth, GameStats.Instance.BoxesInHight);
	}
	
	// Update is called once per frame
	void Update () {
		
	}



    public virtual void MakeTurnByLine(int inIndex)
    {
        int BoxCount = MyBoxes.Count;


        bool AllBoxesFill = true;
        int BoxesFound = 0;

        for(int i = 0; i < BoxCount; ++i)
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

    protected virtual void NewGame()
    {

    }

    protected virtual void GameOver()
    {
        print("GameOver, f/s : " + FirstScore + " : " + SecondScore);
    }
}
