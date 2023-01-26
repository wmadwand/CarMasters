using System;
using System.Collections;
using System.Collections.Generic;
using Technoprosper.Gameplay.Player;
using UnityEngine;

public class LevelFinishTrigger : MonoBehaviour
{
    public static event Action OnFinish;

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();

        if (player && player.IsAlive)
        {
            Debug.LogWarning("Finish level");
            OnFinish?.Invoke();
            Destroy(this);
        }
    }
}
