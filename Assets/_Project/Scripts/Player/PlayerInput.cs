using Technoprosper.Gameplay.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Technoprosper.Input.UI
{
    public class PlayerInput : MonoBehaviour
    {
        public PlayerMovementInput movement;
        public PlayerRotationInput rotation;

        public IEnumerator Init(Player player)
        {
            movement.Init(player);
            rotation.Init(player);

            yield return null;
        }

        public void SetActive(bool value)
        {
            movement.enabled = value;
            rotation.enabled = value;
        }
    }
}