using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingTransition : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(loadSceneAfterSeconds());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator loadSceneAfterSeconds()
    {
        yield return new WaitForSeconds(3f);
        GameManager.Instance.LoadSceneRequest("MainMenuScene");
    }
}
