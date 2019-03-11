using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScore : MonoBehaviour {

    public static PlayerScore instance;
    public int score=0;
    

    public Text scoreTxt;
    
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance);   
    }

    

    void Update()
    {
        if (score > PlayerPrefs.GetInt("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", score);
           
        }

        scoreTxt.text = "Score : " + score;
       

    }

    
}
