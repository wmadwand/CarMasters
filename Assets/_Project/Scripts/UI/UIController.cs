using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public StartGamePanel startGamePanel;
    public FinishGamePanel finishGamePanel;
    public float beforeShowFinishTimer = 2;

    private void Start()
    {
        LevelFinishTrigger.OnFinish += LevelFinishTrigger_OnFinish;
    }

    private void LevelFinishTrigger_OnFinish()
    {
        StartCoroutine(LevelFinishTrigger_OnFinishRoutine());
    }

    private IEnumerator LevelFinishTrigger_OnFinishRoutine()
    {
        yield return new WaitForSeconds(beforeShowFinishTimer);

        finishGamePanel.SetActive(true);
    }

    private void OnDestroy()
    {
        LevelFinishTrigger.OnFinish -= LevelFinishTrigger_OnFinish;
    }
}
