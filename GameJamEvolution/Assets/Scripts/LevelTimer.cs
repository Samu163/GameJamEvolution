using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public float timeRemaining = 20;
    public int minutesRemaining = 0;
    public int secondsRemaining;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timeRemaining >= 60)
        {
            minutesRemaining = Mathf.FloorToInt(timeRemaining / 60);
            secondsRemaining = Mathf.FloorToInt(timeRemaining % 60);
        }
        else
        {
            minutesRemaining = 0;
            secondsRemaining = (int)timeRemaining;
        }

        if (timeRemaining > 0 && !GameManager.Instance.isPaused)
        {
            timeRemaining -= Time.deltaTime;
            timerText.text = string.Format("{0:00}:{1:00}", minutesRemaining, secondsRemaining);
        }
        else
        {
            timerText.text = "00:00";
        }

        if (timeRemaining <= 0)
        {
            LevelManager.Instance.GameOver();
        }
    }
}
