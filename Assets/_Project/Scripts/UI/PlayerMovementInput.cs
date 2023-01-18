using CarMasters.Gameplay.Player;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CarMasters.UI.Input
{
    public class PlayerMovementInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private Player _player;

        //---------------------------------------------------------------

        public void Init(Player player)
        {
            _player = player;
        }

        //---------------------------------------------------------------

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            _player?.Move(true);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            _player?.Move(false);
        }
    }
}