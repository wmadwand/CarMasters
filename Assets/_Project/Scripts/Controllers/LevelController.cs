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

    public void SaveLevelIndex()
    {
        PlayerPrefs.SetInt(LevelIndexKey, _currentLevelIndex);
    }

    //private void LoadLevelIndex()
    //{
    //    _currentLevelIndex = PlayerPrefs.GetInt(LevelIndexKey, 0);
    //}

    public void LoadLevel()
    {
        StartCoroutine(LoadLevelRoutine());
    }

    public IEnumerator LoadLevelRoutine()
    {
        _currentLevelIndex = PlayerPrefs.GetInt(LevelIndexKey, 0);
        var level = levelCollection.GetLevel(_currentLevelIndex);

        var currentLevelObject = Instantiate(level.data.prefab);
        track = currentLevelObject.GetComponent<Track>();

        yield return null;
    }
}