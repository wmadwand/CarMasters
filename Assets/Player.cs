using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerMovement _movement;
    private PlayerGravity _gravity;

    private void Awake()
    {
        _movement = GetComponent<PlayerMovement>();
        _gravity = GetComponent<PlayerGravity>();
    }

    public void SetMove(bool isActive)
    {
        //isMoveButtonPressed = isActive;
    }
}