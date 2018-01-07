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
        GenerateDots();

        GenerateLines();

        GenerateBoxes();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void GenerateBoxes()
    {
        int XBoxes = GameStats.Instance.BoxesInWidth;
        int YBoxes = GameStats.Instance.BoxesInHight;

        float Wide = GameStats.Instance.BoxWide;

        S_Box LocalBox;
       

        for(int y = 0; y < YBoxes; ++y)
            for(int x = 0; x < XBoxes; ++x)
            {
                LocalBox = Instantiate(MyBox, new Vector3(((float)x + 0.5f) * Wide, ((float)y + 0.5f) * Wide, 0f), Quaternion.identity).GetComponent<S_Box>();
                if (LocalBox == null)
                {
                    print("Box generation problem; x/y : " + x + " : " + y);
                    return;
                }
                LocalBox.Index = y * XBoxes + x;
                MyBoxes.Add(LocalBox);
            }
    }

    void GenerateLines()
    {

        int XBoxes = GameStats.Instance.BoxesInWidth;
        int YBoxes = GameStats.Instance.BoxesInHight;

        float Wide = GameStats.Instance.BoxWide;

        S_Line LocalLine;

        // Vertical Lines

        for (int y = 0; y < YBoxes; ++y)
            for(int x = 0; x < XBoxes + 1; ++x)
            {
                LocalLine = Instantiate(MyLine, new Vector3(x * Wide, ((float)y + 0.5f) * Wide, -1f), Quaternion.identity).GetComponent<S_Line>();
                if(LocalLine == null)
                {
                    print("Vertical Line generation problem; x/y : " + x + " : " + y);
                    return;
                }
                LocalLine.Index = y * (XBoxes + 1) + x;
                LocalLine.IsVertical = true;
                MyLines.Add(LocalLine);
            }

        int LastIndex = (XBoxes + 1) * YBoxes;

        // Horizontal Lines

        for(int y = 0; y < YBoxes + 1; ++y)
            for(int x = 0; x < XBoxes; ++x)
            {
                LocalLine = Instantiate(MyLine, new Vector3(((float)x + 0.5f) * Wide, y * Wide, -1f), Quaternion.identity).GetComponent<S_Line>();
                if (LocalLine == null)
                {
                    print("Horizontal Line generation problem; x/y : " + x + " : " + y);
                    return;
                }
                LocalLine.Index = LastIndex + y * XBoxes + x;
                LocalLine.IsVertical = false;
                MyLines.Add(LocalLine);
            }
    }

    void GenerateDots()
    {
        int XBoxes = GameStats.Instance.BoxesInWidth;
        int YBoxes = GameStats.Instance.BoxesInHight;

        float Wide = GameStats.Instance.BoxWide;

        S_Dot LocalDot;

        for (int y = 0; y < YBoxes + 1; ++y)
            for(int x = 0; x < XBoxes + 1; ++x)
            {
                LocalDot = Instantiate(MyDot, new Vector3(x * Wide, y * Wide, -2f), Quaternion.identity).GetComponent<S_Dot>();
                if(LocalDot == null)
                {
                    print("Dot generation problem; x/y : " + x + " : " + y);
                    return;
                }
                LocalDot.Index = y * (XBoxes + 1) + x;
                MyDots.Add(LocalDot);
            }
    }

    public void MakeTurnByLine(int inIndex)
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

    public void MakeTurnByBox(int InIndex)
    {
        
    }

    void GameOver()
    {
        print("GameOver, f/s : " + FirstScore + " : " + SecondScore);
    }
}
