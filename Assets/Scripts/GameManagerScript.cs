using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    //create singleton
    public static GameManagerScript Instance;


    //CREATE LIST OF COINS
    public static List<GameObject> coinList = new();
    private int _coinsToWin;
    public static int score = 0;
    public GameObject winScreen;

    public TextMeshProUGUI scoreText;

    public MetaScript myMeta;

    // Start is called before the first frame update
    private void Start()
    {
        coinList.AddRange(GameObject.FindGameObjectsWithTag("Coin"));

        //singleton code
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public static void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
        Instance.scoreText.text = score.ToString();

        if (coinList.Count == 0) Instance.myMeta.OpenWin();
    }

    public static void WinGame()
    {
        Instance.winScreen.SetActive(true);
    }
}