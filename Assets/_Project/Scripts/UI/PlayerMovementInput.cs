using Technoprosper.Gameplay.Player;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Technoprosper.Input.UI
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
            _player?.SetMove(true);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            _player?.SetMove(false);
        }
    }
}