using System;
using System.Collections;
using System.Collections.Generic;
using Technoprosper.Gameplay.Player;
using UnityEngine;

public class LevelFinishTrigger : MonoBehaviour
{
    public static event Action OnFinish;

    public GameObject effectPrefab;
    public Transform[] effectPoints;

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();

        if (player && player.IsAlive)
        {
            Debug.LogWarning("Finish level");
            PlayEffect();
            OnFinish?.Invoke();
            Destroy(this);
        }
    }

    private void PlayEffect()
    {
        foreach (var item in effectPoints)
        {
            Instantiate(effectPrefab, item);
        }
    }
}
