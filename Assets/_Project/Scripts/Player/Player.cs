using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerMovementNew _movement;
    private PlayerRotationNew _rotation;
    private PlayerGravity _gravity;

    private void Awake()
    {
        _movement = GetComponent<PlayerMovementNew>();
        _rotation = GetComponent<PlayerRotationNew>();
        _gravity = GetComponent<PlayerGravity>();
    }

    public void SetMove(bool isActive)
    {
        _movement?.SetMove(isActive);
        _rotation?.SetMove(isActive);
    }
}