using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour {

    public GameObject GameOver;
    public GameObject GameWin;

    public Text gameover_scoreText,gamewinScoreText;
    public Text gameover_highscoreText,gamewinHighScoreTxt;

    public static UIManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance);
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void GameOver_Finish()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.allClips[2], false);
        GameOver.SetActive(true);
        gameover_scoreText.text = ""+PlayerScore.instance.score;
        gameover_highscoreText.text = "" + PlayerPrefs.GetInt("HighScore");
    }

    public void GameWin_Finish()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.allClips[2], false);
        GameWin.SetActive(true);
        gamewinScoreText.text = "" + PlayerScore.instance.score;
        gamewinHighScoreTxt.text = "" + PlayerPrefs.GetInt("HighScore");
    }

    public void Restart()
    {
        Application.LoadLevel(0);
    }
}
