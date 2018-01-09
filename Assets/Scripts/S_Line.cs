using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Line : MonoBehaviour {

    [HideInInspector] public int Index;

    [HideInInspector] public bool IsVertical;

    [HideInInspector] public int LeftBottomDot;
    [HideInInspector] public int RightUpDot;

    [HideInInspector] private bool isOccupied = false;

    public Color OccupiedColor;

    public bool IsOccupied { get { return isOccupied; } }

    Color StartColor;

    // Use this for initialization
    void Start () {
		if(IsVertical)
        {
            LeftBottomDot = Index;
            RightUpDot = Index + (GameStats.Instance.BoxesInWidth + 1);

            transform.rotation = Quaternion.Euler(0, 0, 90f);
        }
        else
        {
            int z = Index - (GameStats.Instance.BoxesInHight + 1) * GameStats.Instance.BoxesInWidth;
            LeftBottomDot = z / GameStats.Instance.BoxesInWidth + z;

            RightUpDot = LeftBottomDot + 1;
        }

        StartColor = GetComponent<SpriteRenderer>().color;

        transform.localScale = Vector3.one * GameStats.Instance.BoxWide / 5.12f;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnMouseDown()
    {
        if(GameManager.Instance.CanTurn && !GameManager.Instance.OnPause)
        if(Occupy())
        GameManager.Instance.MakeTurnByLine(Index);
    }

    

    public bool Occupy()
    {
        if (!isOccupied)
        {
            isOccupied = true;
            
            GetComponent<SpriteRenderer>().color = Color.cyan;
            return true;
        }
        return false;
    }

    public bool HardOccupy(bool inOccupy = true)
    {

        if (inOccupy != IsOccupied)
        {
            GetComponent<SpriteRenderer>().color = inOccupy ? Color.cyan : StartColor;
            isOccupied = inOccupy;
            return true;
        }
        return false;
    }
}
