using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator {

    public static GameObject MyBox;
    public static GameObject MyLine;
    public static GameObject MyDot;

    public static void GenerateGrid(out List<S_Dot> InDots, out List<S_Line> InLines, out List<S_Box> InBoxes, float inWide, int Boxes_X, int Boxes_Y, GameObject inBox, GameObject inLine, GameObject inDot)
    {
        MyDot = inDot;
        MyLine = inLine;
        MyBox = inBox;

        InDots = GenerateDots(inWide, Boxes_X, Boxes_Y);
        InLines = GenerateLines(inWide, Boxes_X, Boxes_Y);
        InBoxes = GenerateBoxes(inWide, Boxes_X, Boxes_Y);
        
    }

    private static List<S_Dot> GenerateDots(float inWide, int Boxes_X, int Boxes_Y)
    {
        List<S_Dot> MyDots = new List<S_Dot>();

        S_Dot LocalDot;

        for (int y = 0; y < Boxes_Y + 1; ++y)
            for (int x = 0; x < Boxes_X + 1; ++x)
            {
                LocalDot = GameObject.Instantiate(MyDot, new Vector3(x * inWide, y * inWide, -2f), Quaternion.identity).GetComponent<S_Dot>();
                if (LocalDot == null)
                {
                    MonoBehaviour.print("Dot generation problem; x/y : " + x + " : " + y);
                    return null;
                }
                LocalDot.Index = y * (Boxes_X + 1) + x;
                MyDots.Add(LocalDot);
            }

        return MyDots;
    }

    private static List<S_Line> GenerateLines(float inWide, int Boxes_X, int Boxes_Y)
    {
        List<S_Line> MyLines = new List<S_Line>();
        S_Line LocalLine;

        // Vertical Lines

        for (int y = 0; y < Boxes_Y; ++y)
            for (int x = 0; x < Boxes_X + 1; ++x)
            {
                LocalLine = GameObject.Instantiate(MyLine, new Vector3(x * inWide, ((float)y + 0.5f) * inWide, -1f), Quaternion.identity).GetComponent<S_Line>();
                if (LocalLine == null)
                {
                    MonoBehaviour.print("Vertical Line generation problem; x/y : " + x + " : " + y);
                    return null;
                }
                LocalLine.Index = y * (Boxes_X + 1) + x;
                LocalLine.IsVertical = true;
                MyLines.Add(LocalLine);
            }

        int LastIndex = (Boxes_X + 1) * Boxes_Y;

        // Horizontal Lines

        for (int y = 0; y < Boxes_Y + 1; ++y)
            for (int x = 0; x < Boxes_X; ++x)
            {
                LocalLine = GameObject.Instantiate(MyLine, new Vector3(((float)x + 0.5f) * inWide, y * inWide, -1f), Quaternion.identity).GetComponent<S_Line>();
                if (LocalLine == null)
                {
                    MonoBehaviour.print("Horizontal Line generation problem; x/y : " + x + " : " + y);
                    return null;
                }
                LocalLine.Index = LastIndex + y * Boxes_X + x;
                LocalLine.IsVertical = false;
                MyLines.Add(LocalLine);
            }

        return MyLines;
    }

    private static List<S_Box> GenerateBoxes(float inWide, int Boxes_X, int Boxes_Y)
    {

        S_Box LocalBox;

        List<S_Box> MyBoxes = new List<S_Box>();

        for (int y = 0; y < Boxes_Y; ++y)
            for (int x = 0; x < Boxes_X; ++x)
            {
                LocalBox = GameObject.Instantiate(MyBox, new Vector3(((float)x + 0.5f) * inWide, ((float)y + 0.5f) * inWide, 0f), Quaternion.identity).GetComponent<S_Box>();
                if (LocalBox == null)
                {
                    MonoBehaviour.print("Box generation problem; x/y : " + x + " : " + y);
                    return null;
                }
                LocalBox.Index = y * Boxes_X + x;
                MyBoxes.Add(LocalBox);
            }

        return MyBoxes;
    }
}
