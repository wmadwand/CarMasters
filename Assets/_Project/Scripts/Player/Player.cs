using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerMovement _movement;
    private PlayerRotation _rotation;
    private PlayerGravity _gravity;
    private PlayerHealth _health;

    private void Awake()
    {
        _movement = GetComponent<PlayerMovement>();
        _rotation = GetComponent<PlayerRotation>();
        _gravity = GetComponent<PlayerGravity>();
        _health = GetComponent<PlayerHealth>();
    }

    public void Move(bool value)
    {
        if (_health && !_health.IsAlive)
        {
            return;
        }

        _movement?.Move(value);
        _rotation?.Move(value);
    }

    public void RotateBy(float value)
    {
        _rotation?.RotateBy(value);
    }
}