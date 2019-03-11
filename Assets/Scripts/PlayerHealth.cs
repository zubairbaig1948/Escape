using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {
    public float totalHealth=100;
    public float healthPercentage=0.0f;
    public Image healthBar;
    public Text healthText;

    void Start()
    {
        healthText.text = "Health :" + totalHealth;

        healthBar.fillAmount = totalHealth/100;
    }

    public void TakeDamage(int amount)
    {
        if(totalHealth>0)
        {
            totalHealth = totalHealth-amount;
            healthPercentage = totalHealth/100;

            Debug.LogError("totalHealth" + totalHealth + "Percentage" + healthPercentage);

            if (totalHealth == 0)
            {
                UIManager.instance.GameOver_Finish();
            }

        }
        else
        {

            UIManager.instance.GameOver_Finish();
        }

        healthText.text = "Health :" + healthPercentage*100;

        healthBar.fillAmount = healthPercentage;
        



        
    }
}
