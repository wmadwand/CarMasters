using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerMovement _movement;
    private PlayerRotation02 _rotation;
    private PlayerGravity _gravity;

    private void Awake()
    {
        _movement = GetComponent<PlayerMovement>();
        _rotation = GetComponent<PlayerRotation02>();
        _gravity = GetComponent<PlayerGravity>();
    }

    public void SetMove(bool isActive)
    {
        _movement?.SetMove(isActive);
        _rotation?.SetMove(isActive);
    }
}