using Dreamteck.Splines;
using UnityEngine;

namespace CarMasters.Gameplay.Player
{
    public class Player : MonoBehaviour
    {
        //TODO: a couple of dependencies !
        private PlayerMovement _movement;
        private PlayerRotation _rotation;
        private PlayerGravity _gravity;
        private PlayerHealth _health;

        //---------------------------------------------------------------

        public SplineProjector SplineProjector => _rotation.SplineProjector;

        public void SetMove(bool value)
        {
            _movement?.SetMove(value);
            _rotation?.SetMove(value);
        }


        //TODO: Disable PlayerInput + PlayerInputPanel
        public void Stop()
        {
            if (_health && !_health.IsAlive)
            {
                _movement.StopRightThere();
                _movement.Stop();
                _movement.enabled = false;

                _movement?.SetMove(false);
                _rotation?.SetMove(false);

                return;
            }
        }

        public void RotateBy(float value)
        {
            if (_health && !_health.IsAlive)
            {
                _rotation?.SetMove(false);
                _rotation?.RotateBy(0);
                _rotation.enabled = false;

                return;
            }

            _rotation?.RotateBy(value);
        }

        //---------------------------------------------------------------

        private void Init()
        {

        }

        private void Awake()
        {
            _movement = GetComponent<PlayerMovement>();
            _rotation = GetComponent<PlayerRotation>();
            _gravity = GetComponent<PlayerGravity>();
            _health = GetComponent<PlayerHealth>();
        }
    }
}