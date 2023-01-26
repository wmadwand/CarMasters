using System;
using System.Collections;
using System.Collections.Generic;
using Technoprosper.Gameplay.Player;
using UnityEngine;

public class TrackPartFinishTrigger : MonoBehaviour
{
    public static event Action OnPlayerEnter;

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();

        if (player && player.IsAlive)
        {
            OnPlayerEnter?.Invoke();
            Destroy(this);
        }
    }
}