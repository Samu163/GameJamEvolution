using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsButtons : MonoBehaviour
{

    public Button returnButton;

    private void Awake()
    {
        returnButton.onClick.AddListener(() => GameManager.Instance.LoadScreenRequest("PauseCanvas"));
        returnButton.onClick.AddListener(() => GameManager.Instance.DestroyScreenRequest("SettingsCanvas"));
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
