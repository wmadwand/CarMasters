using Technoprosper.Gameplay.Player;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Technoprosper.Input.UI
{
    public class PlayerRotationInput : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        private Player _player;
        private Vector2 _startDragPosition;
        private float _delta;

        //---------------------------------------------------------------

        public void Init(Player player)
        {
            _player = player;
        }

        //---------------------------------------------------------------

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            _startDragPosition = eventData.pressPosition;
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            var dragPosition = eventData.position;
            var direction = dragPosition - _startDragPosition;

            //TODO: sqrMagnitude !!!
            _delta = direction.magnitude * eventData.delta.normalized.x;
            _player?.RotateBy(_delta);
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            _delta = 0;
        }
    }
}