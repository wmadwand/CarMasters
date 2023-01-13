using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerMovement _movement;
    private PlayerRotation _rotation;
    private PlayerGravity _gravity;

    private void Awake()
    {
        _movement = GetComponent<PlayerMovement>();
        _rotation = GetComponent<PlayerRotation>();
        _gravity = GetComponent<PlayerGravity>();
    }

    public void SetMove(bool isActive)
    {
        _movement?.SetMove(isActive);
        _rotation?.SetMove(isActive);
    }
}