using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuHud : MonoBehaviour {

    public GameObject StartMenu;
    public GameObject ClientMenu;
    public GameObject ServerMenu;
    public GameObject OfflineMenu;

    public Dropdown[] MyGridDDs;

    private void Start()
    {
        LoadStartMenu();
    }

    // on grid changing
    public void ChooseGrid(int inGrid)
    {
        GameStats.Instance.BoxesInWidth = inGrid + 2;
        GameStats.Instance.BoxesInHight = inGrid + 2;

        Dropdown[] MyDD = GetComponents<Dropdown>();

        foreach (Dropdown dd in MyDD)
            dd.value = inGrid;
    }

    public void StartOffline()
    {
        // do not start if colors did not choosed
        if (GameStats.Instance.FirstColor != Color.clear && GameStats.Instance.SecondColor != Color.clear)
            SceneManager.LoadScene(1);
    }

    public void SetIP(string IP)
    {
        GameStats.Instance.IP = IP;
        print(GameStats.Instance.IP);
    }
    public void SetPort(string Port)
    {
        GameStats.Instance.PORT = int.Parse(Port);
        
    }

    public void StartClient()
    {
        // do not start if colors did not choosed or ip and port sets up incorrectly
        if (GameStats.Instance.FirstColor != Color.clear && GameStats.Instance.SecondColor != Color.clear)
            if (GameStats.Instance.IP != "" && GameStats.Instance.PORT > 999 && GameStats.Instance.PORT < 10000)
            {
            GameStats.Instance.isServer = false;
            SceneManager.LoadScene(2);
            }        
    }

    public void StartServer()
    {
        // do not start if colors did not choosed or port sets up incorrectly
        if (GameStats.Instance.FirstColor != Color.clear && GameStats.Instance.SecondColor != Color.clear)
            if (GameStats.Instance.PORT > 999 && GameStats.Instance.PORT < 10000)
            {
            GameStats.Instance.isServer = true;
                GameStats.Instance.IP = "";
            SceneManager.LoadScene(2);
            }
    }

    // sets colors, ip, port clear when we returning back on main menu
    public void LoadStartMenu()
    {
        GameStats.Instance.FirstColor = Color.clear;
        GameStats.Instance.SecondColor = Color.clear;

        ColorChooser[] MyCCs = FindObjectsOfType<ColorChooser>();
        foreach (ColorChooser cc in MyCCs)
            cc.DeChooseColor();

        GameStats.Instance.IP = "";
        GameStats.Instance.PORT = 0;

        foreach (Dropdown dd in MyGridDDs)
            dd.value = GameStats.Instance.BoxesInWidth - 2;

        ClientMenu.SetActive(false);
        ServerMenu.SetActive(false);
        OfflineMenu.SetActive(false);
        StartMenu.SetActive(true);
    }
    public void LoadClientMenu()
    {
        ClientMenu.SetActive(true);
        ServerMenu.SetActive(false);
        OfflineMenu.SetActive(false);
        StartMenu.SetActive(false);
    }
    public void LoadServerMenu()
    {
        foreach (Dropdown dd in MyGridDDs)
            dd.value = GameStats.Instance.BoxesInWidth - 2;

        ClientMenu.SetActive(false);
        ServerMenu.SetActive(true);
        OfflineMenu.SetActive(false);
        StartMenu.SetActive(false);
    }
    public void LoadOfflineMenu()
    {
        foreach (Dropdown dd in MyGridDDs)
            dd.value = GameStats.Instance.BoxesInWidth - 2;

        ClientMenu.SetActive(false);
        ServerMenu.SetActive(false);
        OfflineMenu.SetActive(true);
        StartMenu.SetActive(false);
    }
}
