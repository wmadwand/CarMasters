using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Technoprosper.Gameplay.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        public bool IsAlive { get; private set; } = true;

        public void Restore()
        {

        }

        //TODO: run event OnPlayerDead; 
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<DeadlyObstacle>())
            {
                IsAlive = false;

                var player = GetComponent<Player>();
                player.Respawn(other.transform.position);
            }
        }
    }
}