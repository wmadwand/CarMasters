using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInputPanelHelper : MonoBehaviour, IPointerDownHandler
{
    public StartGamePanel startGamePanel;

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        startGamePanel.SetActive(false);
    }
}