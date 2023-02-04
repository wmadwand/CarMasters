using System.Collections;
using System.Collections.Generic;
using Technoprosper.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoSingleton<SceneLoader>
{
    public GameObject loadingPanel;
    public string levelSceneName = "Main";
    public string loaderSceneName = "Loader";

    //---------------------------------------------------------------

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

        var levelScene = SceneManager.GetSceneByName(levelSceneName);
        AsyncOperation asyncUnload = null;
        if (levelScene.isLoaded)
        {
            asyncUnload = SceneManager.UnloadSceneAsync(levelSceneName, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
        }

        if (asyncUnload != null)
        {
            //yield return new WaitUntil(() => asyncUnload.isDone);
            yield return asyncUnload;
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelSceneName, LoadSceneMode.Additive);

        //yield return new WaitUntil(() => asyncLoad.isDone);
        yield return asyncLoad;

        //TODO: use SetActiveScene
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelSceneName));
        yield return GameController.Instance.StartGameRoutine();

        //SceneManager.SetActiveScene(SceneManager.GetSceneByName("Scene2"));
        //SceneManager.MoveGameObjectToScene

        loadingPanel.SetActive(false);
    }
}