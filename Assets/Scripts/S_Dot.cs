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
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnMouseDown()
    {
        print("Dot clicked : " + Index);
    }
}
