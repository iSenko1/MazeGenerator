using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;


    private float startTime;
    public bool gameIsRunning = true;
    public TMP_Text scoreText;
    public TMP_Text highscoreText;
    public TMP_Text timerText;

    int score = 0;
    int highscore = 0;
    private float timePlayed;
    private float gameStartTime; // Renamed for clarity.


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //startTime = Time.time;
        gameStartTime = Time.time;
        //highscore = PlayerPrefs.GetInt("highscore", 0);
        scoreText.text = score.ToString() + " COINS";
        //highscoreText.text = "HIGHSCORE: " + highscore.ToString();

        //float highscore = PlayerPrefs.GetFloat("Highscore", float.MaxValue);
        float highscore = PlayerPrefs.GetFloat("Highscore", float.MaxValue);
        if (highscore != float.MaxValue)
        {
            highscoreText.text = "HIGHSCORE: " + FormatTime(highscore);
        }
    }
    public string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time % 60;
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void AddPoint()
    {
        score += 1;
        scoreText.text = score.ToString() + " COINS";
        
        //if(highscore < score)
        //    PlayerPrefs.SetInt("highscore", score);
        /*
        float currentHighscore = PlayerPrefs.GetFloat("Highscore", float.MaxValue);
        if (timePlayed < currentHighscore)
        {
            PlayerPrefs.SetFloat("Highscore", timePlayed);
            Debug.Log("New highscore set: " + timePlayed);
        }
        */

    }

    void Update()
    {
        if (!gameIsRunning) return;
        timePlayed = Time.time - gameStartTime;

        int minutes = (int)timePlayed / 60;
        int seconds = (int)timePlayed % 60;
        timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
    }

}
