using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {

    public Image FirstImage;
    public Image SecondImage;
    public Text FirstText;
    public Text SecondText;

    public GameObject PauseButton;
    public GameObject PauseMenu;

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

    public void UpdateHUD()
    {
        FirstText.text = GameStats.Instance.FirstWins.ToString();
        SecondText.text = GameStats.Instance.SecondWins.ToString();
    }

    public void GoPause()
    {
        PauseButton.SetActive(false);
        PauseMenu.SetActive(true);
        GameManager.Instance.OnPause = true;
    }
    public void GoUnpause()
    {

        PauseButton.SetActive(true);
        PauseMenu.SetActive(false);
        GameManager.Instance.OnPause = false;
    }
}
