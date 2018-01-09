using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStats : MonoBehaviour {

    [HideInInspector] public int FirstWins;
    [HideInInspector] public int SecondWins;

    static GameStats instance = null;

    public int BoxesInHight = 1;
    public int BoxesInWidth = 2;

    public Color FirstColor;
    public Color SecondColor;

    public bool isServer = true;

    public string IP;
    public int PORT;

    public float BoxWide = 5.12f;

    [HideInInspector] public bool isFirstTurn = true;

    public static GameStats Instance
    {
        get
        {
            if (instance == null)
                instance = new GameObject().AddComponent<GameStats>();
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
              {
                Destroy(gameObject);
                return;
              }

        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
