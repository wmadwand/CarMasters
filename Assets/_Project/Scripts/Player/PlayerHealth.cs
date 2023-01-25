using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Technoprosper.Gameplay.Player
{
    public enum DeathCause
    {
        Crash,
        FallDown
    }

    public class PlayerHealth : MonoBehaviour
    {
        public bool IsAlive { get; private set; } = true;
        public Vector3 fallDownOffset = new Vector3(5, -5, 5);
        public float waitforFallingTimer = 2;

        private Player _player;
        private bool _hasFallenDown = false;
        private float _timer = 0.0f;

        //---------------------------------------------------------------

        public void Restore()
        {
            IsAlive = true;
            _hasFallenDown = false;
        }

        //TODO: run event OnPlayerDead; 
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<DeadlyObstacle>())
            {
                IsAlive = false;
                _player.Respawn(other.transform.position, DeathCause.Crash);
            }
        }

        private void Awake()
        {
            _player = GetComponent<Player>();
        }

        private void Update()
        {
            if (!_player.GetComponent<PlayerGravity>().IsGrounded && !_hasFallenDown)
            {
                var playerPos = _player.transform.position;
                var splinePos = _player.SplineProjector.result.position;
                var offset = playerPos - splinePos;

                if (offset.y < fallDownOffset.y)
                {
                    _timer += Time.deltaTime;

                    if (_timer >= waitforFallingTimer)
                    {
                        _hasFallenDown = true;
                        IsAlive = false;
                        //_player.SplineProjector.spline.
                        _timer = 0;
                        var deadPostion = _player.SplineProjector.result.position;
                        _player.Respawn(deadPostion, DeathCause.FallDown);
                    }
                }
            }
        }
    }
}