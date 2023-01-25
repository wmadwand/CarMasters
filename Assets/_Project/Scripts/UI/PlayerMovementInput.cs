using Technoprosper.Gameplay.Player;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Technoprosper.Input.UI
{
    public class PlayerMovementInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public GameObject startGamePanel;
        public GameObject startGamePanel2;

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

            //TODO: remove from here
            if (startGamePanel.activeInHierarchy)
            {
                startGamePanel.SetActive(false);
                startGamePanel2.SetActive(false);
            }            
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            _player?.SetMove(false);
        }
    }
}