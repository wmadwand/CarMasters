using CarMasters.Gameplay.Player;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CarMasters.UI.Input
{
    public class PlayerMovementInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
       public Player _player;

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            _player.Move(true);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            _player.Move(false);
        }
    }
}