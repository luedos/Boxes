﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    static GameManager instance = null;
    
    [HideInInspector] public List<S_Box> MyBoxes = new List<S_Box>();           // All boxes on the level
    [HideInInspector] public List<S_Line> MyLines = new List<S_Line>();         // All lines on the level
    [HideInInspector] public List<S_Dot> MyDots = new List<S_Dot>();            // All dots on the level

    public GameObject MyBox;
    public GameObject MyLine;
    public GameObject MyDot;
    public Camera MyCamera;
    public GameObject GameOverScreen;
    public GameObject MyHUD;

    [HideInInspector] public bool OnPause = false;
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

    public virtual void NewGame()
    {

    }

    protected virtual void GameOver()
    {
        print("GameOver, f/s : " + FirstScore + " : " + SecondScore);
    }
}
