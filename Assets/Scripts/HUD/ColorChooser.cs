using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ColorChooser : MonoBehaviour {

    public bool isFirst;
    [HideInInspector] public Color MyColor;
    public int ColorIndex;

    private void Start()
    {
        MyColor = GetComponent<Image>().color;
    }

    // if we choosed, dechoose every other color on our player and set us as color on GameStates
    public void OnColorChoosed()
    {
        if(MyColor != (isFirst ? GameStats.Instance.SecondColor : GameStats.Instance.FirstColor))
        {
            if (isFirst)
                GameStats.Instance.FirstColor = MyColor;
            else
                GameStats.Instance.SecondColor = MyColor;

            GetComponent<Image>().color = MyColor / 2;


            ColorChooser[] MyChoosers = FindObjectsOfType<ColorChooser>();

            foreach (ColorChooser cc in MyChoosers)
            {
                if (cc.isFirst == isFirst && cc != this)
                    cc.DeChooseColor();

            }
        }
    }

    public void DeChooseColor()
    {
        GetComponent<Image>().color = MyColor;
    }

    public void HardChoose()
    {
        GetComponent<Image>().color = MyColor / 2;
    }
}
