using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "LevelCollection", menuName = "CarMasters/LevelCollection")]
public class LevelCollectionData : ScriptableObject
{
    public LevelData defaultLevel;
    public List<LevelData> levels;

    public LevelData GetLevelSafe(int index)
    {
        index = Math.Clamp(index, 0, levels.Count - 1);
        return levels[index];
    }
}