using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    float delta;

    public RotationHandlerTest test;

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        delta = 0;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        delta += eventData.delta.normalized.x;

        Debug.Log($"delta {delta}");
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        delta = 0;
    }

    private void Update()
    {
        test.SetXInput(delta);
    }
}
