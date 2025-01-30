using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NavigationController : MonoBehaviour
{
    public List<string> sceneNames;
    public List<GameObject> screenPrefabs;
    public List<GameObject> activeScreens;
    public void LoadScene(string sceneName)
    {
        int index = sceneNames.IndexOf(sceneName);
        if (index != -1)
        {
            StartCoroutine(LoadYourAsyncScene(sceneName));
        }
    }

    IEnumerator LoadYourAsyncScene(string sceneName)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void LoadScreen(string screenName, Transform parent)
    {
        var screenToInstantiate = screenPrefabs.Find(screen => screen.name == screenName);
        if (screenToInstantiate != null)
        {
            var screen = Instantiate(screenToInstantiate);
            screen.transform.SetParent(parent, false);
            this.activeScreens.Add(screen);
        }
    }

    public void DestroyScreen(string screenName)
    {
        screenName = screenName + "(Clone)";
        if (activeScreens.Find(screen => screen.name.StartsWith(screenName)) != null)
        {
            Destroy(activeScreens.Find(screen => screen.name == screenName));
        }
        activeScreens.Remove(activeScreens.Find(screen => screen.name == screenName));
    }
}
