using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public GameObject loadingPanel;

    private void Start()
    {
        LoadNextLevel();
    }

    public void LoadNextLevel()
    {
        loadingPanel.SetActive(true);
        StartCoroutine(LoadYourAsyncScene());
    }

    IEnumerator LoadYourAsyncScene()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Main", LoadSceneMode.Additive);

        yield return new WaitUntil(() => asyncLoad.isDone);

        //SceneManager.SetActiveScene(SceneManager.GetSceneByName("Scene2"));

        loadingPanel.SetActive(false);
    }
}