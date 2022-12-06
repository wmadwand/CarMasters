using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpeedButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public MyGravity01 player;

    public void OnPointerDown(PointerEventData eventData)
    {
        player.PressMoveButton(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        player.PressMoveButton(false);
    }
}