using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.UI.Input
{
    public class PlayerRotationInput : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public PlayerRotation _player;

        private Vector2 _startDragPosition;
        private float _delta;

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