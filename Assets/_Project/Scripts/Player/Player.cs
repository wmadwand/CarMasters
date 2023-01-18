using Dreamteck.Splines;
using System;
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

        public Action<Vector3> respawnCallback;
        public GameObject explosionPrefab;

        //---------------------------------------------------------------

        public SplineProjector SplineProjector => _rotation.SplineProjector;

        public void SetMove(bool value)
        {
            _movement?.SetMove(value);
            _rotation?.SetMove(value);
        }

        public void ActivateMovement()
        {
            _movement.enabled = true;
            _rotation.enabled = true;
        }

        //TODO: Disable PlayerInput + PlayerInputPanel
        public void Stop()
        {
            _movement.StopRightThere();
            _movement.Stop();
            _movement.enabled = false;

            _movement?.SetMove(false);
            _rotation?.SetMove(false);

            _rotation?.RotateBy(0);
            _rotation.enabled = false;
        }

        public void RotateBy(float value)
        {
            _rotation?.RotateBy(value);
        }

        public void Respawn(Vector3 deadPosition)
        {
            Stop();
            respawnCallback(deadPosition);
            Instantiate(explosionPrefab, transform);
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