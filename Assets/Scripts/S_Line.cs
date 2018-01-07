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

        transform.localScale = Vector3.one * GameStats.Instance.BoxWide / 5.12f;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnMouseDown()
    {
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

    public bool HardOccupy()
    {
        GetComponent<SpriteRenderer>().color = Color.cyan;

        if (!isOccupied)
        {
            isOccupied = true;
            return true;
        }
        return false;
    }
}
