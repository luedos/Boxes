using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour {

    public Image FirstImage;
    public Image SecondImage;
    public Text FirstText;
    public Text SecondText;

    private void Awake()
    {
        FirstImage.color = GameStats.Instance.FirstColor;
        SecondImage.color = GameStats.Instance.SecondColor;
        FirstText.text = GameStats.Instance.FirstWins.ToString();
        SecondText.text = GameStats.Instance.SecondWins.ToString();

    }
}
