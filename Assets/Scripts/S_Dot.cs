using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Dot : MonoBehaviour {

    [HideInInspector] public int Index;

    public Color OccupiedColor;

    // Use this for initialization
    void Start () {
        transform.localScale = Vector3.one * GameStats.Instance.BoxWide / 5.12f;
	}


    private void OnMouseDown()
    {
        print("Dot clicked : " + Index);
    }
}
