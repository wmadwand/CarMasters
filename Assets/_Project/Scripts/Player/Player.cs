using UnityEngine;

namespace Game.Gameplay.Player
{
    public class Player : MonoBehaviour
    {        
        //TODO: a couple of dependencies !
        private PlayerMovement _movement;
        private PlayerRotation _rotation;
        private PlayerGravity _gravity;
        private PlayerHealth _health;

        //---------------------------------------------------------------

        public void Move(bool value)
        {
            if (_health && !_health.IsAlive)
            {
                value = false;
            }

            _movement?.Move(value);
            _rotation?.Move(value);
        }

        public void RotateBy(float value)
        {
            if (_health && !_health.IsAlive)
            {
                value = 0f;
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