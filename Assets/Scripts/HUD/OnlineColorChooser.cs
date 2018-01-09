using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class MyEvent : UnityEvent <int> { }

public class OnlineColorChooser : MonoBehaviour {

    public MyEvent OnColorChoosed;

    public int ColorIndex;

    [HideInInspector] public Color MyColor;
    [HideInInspector] public bool IsOccupied;
    [HideInInspector] public bool isFirst;

    public GameObject BackGroundImage;

    private void Start()
    {
        MyColor = GetComponent<Image>().color;
        BackGroundImage.SetActive(false);
    }

    public void OccupyByPlayer(bool inIsFirst)
    {
        if (inIsFirst)
            BackGroundImage.SetActive(true);

        IsOccupied = true;
        isFirst = inIsFirst;

        GetComponent<Image>().color = MyColor / 2;
    }

    public void Disoccupy()
    {
        IsOccupied = false;
        BackGroundImage.SetActive(false);
        GetComponent<Image>().color = MyColor;
    }

    void ChooseColor()
    {
        if (!IsOccupied)
        {
            OnColorChoosed.Invoke(ColorIndex);
            OccupyByPlayer(true);
        }
    }
}
