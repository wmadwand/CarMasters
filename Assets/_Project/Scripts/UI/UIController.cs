using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public StartGamePanel startGamePanel;
    public FinishGamePanel finishGamePanel;

    private void Start()
    {
        LevelFinishTrigger.OnFinish += LevelFinishTrigger_OnFinish;
    }

    private void LevelFinishTrigger_OnFinish()
    {
        finishGamePanel.SetActive(true);
    }

    private void OnDestroy()
    {
        LevelFinishTrigger.OnFinish -= LevelFinishTrigger_OnFinish;
    }
}
