using CarMasters.Gameplay.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CarMasters.UI.Input
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
    }
}