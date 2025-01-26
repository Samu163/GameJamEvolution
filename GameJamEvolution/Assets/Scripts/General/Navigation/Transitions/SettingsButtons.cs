using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsButtons : MonoBehaviour
{

    public Button returnButton;

    public GameObject pauseCanvas;

    private void Awake()
    {
        returnButton.onClick.AddListener(() => pauseCanvas.SetActive(true));
        returnButton.onClick.AddListener(() => gameObject.SetActive(false));
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
