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

    public void SetMove(bool isActive)
    {
        if (_health && !_health.IsAlive)
        {
            return;
        }

        _movement?.SetMove(isActive);
        _rotation?.SetMove(isActive);
    }
}