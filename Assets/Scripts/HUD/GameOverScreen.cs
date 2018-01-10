using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour {

    public Image FirstImage;
    public Image SecondImage;
    public Text FirstText;
    public Text SecondText;

    // seting up some variables
    public void OnEnable()
    {
        FirstImage.color = GameStats.Instance.FirstColor;
        SecondImage.color = GameStats.Instance.SecondColor;
        FirstText.text = GameStats.Instance.FirstWins.ToString();
        SecondText.text = GameStats.Instance.SecondWins.ToString();

    }
   
}
