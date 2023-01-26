using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public LevelCollectionData levelCollection;

    public Track Track => track;

    private int _currentLevelIndex = 0;

    private const string LevelIndexKey = "LevelIndexKey";
    private Track track;

    public void SaveNextLevel()
    {
        _currentLevelIndex = PlayerPrefs.GetInt(LevelIndexKey, 0);
        _currentLevelIndex++;
        PlayerPrefs.SetInt(LevelIndexKey, _currentLevelIndex);
    }

    public void ResetGame()
    {
        PlayerPrefs.SetInt(LevelIndexKey, 0);
    }

    public IEnumerator LoadLevelRoutine(Transform levelParent)
    {
        _currentLevelIndex = PlayerPrefs.GetInt(LevelIndexKey, 0);
        var level = levelCollection.GetLevel(_currentLevelIndex);

        var currentLevelObject = Instantiate(level.data.prefab, levelParent);
        track = currentLevelObject.GetComponent<Track>();

        yield return new WaitUntil(() => track);
    }
}