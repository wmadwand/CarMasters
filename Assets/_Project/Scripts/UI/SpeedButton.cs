using UnityEngine;
using UnityEngine.EventSystems;

public class SpeedButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Player player;

    public void OnPointerDown(PointerEventData eventData)
    {
        player.SetMove(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        player.SetMove(false);
    }
}
