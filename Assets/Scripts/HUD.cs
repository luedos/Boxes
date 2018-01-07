using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {

    public Image FirstImage;
    public Image SecondImage;

    bool IsFirst;

	// Use this for initialization
	void Start () {

        IsFirst = GameManager.Instance.IsFirstTurn;

        FirstImage.color = IsFirst ? GameStats.Instance.FirstColor : GameStats.Instance.FirstColor * 0.6f;

        SecondImage.color = IsFirst ? GameStats.Instance.SecondColor * 0.6f : GameStats.Instance.SecondColor;

    }
	
	// Update is called once per frame
	void Update () {
		
        if(IsFirst != GameManager.Instance.IsFirstTurn)
        {
            IsFirst = GameManager.Instance.IsFirstTurn;

            FirstImage.color = IsFirst ? GameStats.Instance.FirstColor : GameStats.Instance.FirstColor * 0.6f;

            SecondImage.color = IsFirst ? GameStats.Instance.SecondColor * 0.6f : GameStats.Instance.SecondColor;
        }

	}
}
