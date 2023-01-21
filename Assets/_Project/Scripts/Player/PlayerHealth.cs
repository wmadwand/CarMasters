using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Technoprosper.Gameplay.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        public bool IsAlive { get; private set; } = true;
        public Vector3 offsetToCheck = new Vector3(5, -5, 5);
        public float waitforFallingTimer = 2;

        private Player _player;

        //---------------------------------------------------------------

        public void Restore()
        {
            IsAlive = true;
            hasFallenDown = false;
        }

        //TODO: run event OnPlayerDead; 
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<DeadlyObstacle>())
            {
                IsAlive = false;
                _player.Respawn(other.transform.position);
            }
        }

        private void Awake()
        {
            _player = GetComponent<Player>();
        }

        bool hasFallenDown = false;
        float timer = 0.0f;

        private void Update()
        {
            if (!_player.GetComponent<PlayerGravity>().IsGrounded && !hasFallenDown)
            {
                var playerPos = _player.transform.position;
                var splinePos = _player.SplineProjector.result.position;
                var offset = playerPos - splinePos;

                if (offset.y < offsetToCheck.y)
                {
                    timer += Time.deltaTime;

                    if (timer >= waitforFallingTimer)
                    {
                        hasFallenDown = true;
                        //_player.SplineProjector.spline.
                        timer = 0;
                        var deadPostion = _player.SplineProjector.result.position;
                        _player.Respawn(deadPostion, false);
                    }
                }
            }
        }
    }
}