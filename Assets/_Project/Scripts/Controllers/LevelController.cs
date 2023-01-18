using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public LevelCollectionData levelCollection;
    private int _currentLevelIndex = 0;

    private const string LevelIndexKey = "LevelIndexKey";

    public void SaveLevelIndex()
    {
        PlayerPrefs.SetInt(LevelIndexKey, _currentLevelIndex);
    }

    public void LoadLevelIndex()
    {
        _currentLevelIndex = PlayerPrefs.GetInt(LevelIndexKey, 0);
    }
}
