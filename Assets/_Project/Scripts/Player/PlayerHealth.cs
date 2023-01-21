using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Technoprosper.Gameplay.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        public PlayerSpawner carSpawner;
        public bool IsAlive => _isAlive;



        private bool _isAlive = true;

        public void Restore()
        {

        }


        //TODO: run event OnPlayerDead; 
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<DeadlyObstacle>())
            {
                _isAlive = false;

                var player = GetComponent<Player>();
                player.Respawn(other.transform.position);
            }
        }
    }
}