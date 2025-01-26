using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCanvas : MonoBehaviour
{
    public GameObject pauseCanvas;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !GameManager.Instance.isPaused)
        {
            GameManager.Instance.PauseGame();
            pauseCanvas.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && GameManager.Instance.isPaused)
        {
            GameManager.Instance.ResumeGame();
            pauseCanvas.SetActive(false);
        }
    }
}
