using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CarMasters.Gameplay.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        public CarSpawner carSpawner;
        public bool IsAlive => _isAlive;

        private bool _isAlive = true;

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<DeadlyObstacle>())
            {
                _isAlive = false;

                //1. Disable the car
                //    2. Call CarSpawner
                carSpawner.Respawn(other.transform.position, gameObject);
                //gameObject.SetActive(false);
            }
        }
    } 
}