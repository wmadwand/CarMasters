using UnityEngine;
using UnityEngine.EventSystems;

public class SpeedButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Player player;

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        player.SetMove(true);
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        player.SetMove(false);
    }
}