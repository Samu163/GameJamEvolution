using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCanvas : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !GameManager.Instance.isPaused)
        {
            GameManager.Instance.PauseGame();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && GameManager.Instance.isPaused)
        {
            GameManager.Instance.ResumeGame();
        }
    }
}
