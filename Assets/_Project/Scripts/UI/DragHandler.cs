using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    float delta;

    public RotationHandlerTest test;
    public PlayerRotationNew player;

    private Vector2 startPress;

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        startPress = eventData.pressPosition;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        var currentPosition = eventData.position;
        var direction = currentPosition - startPress;

        //TODO: sqrMagnitude !!!
        delta = direction.magnitude * eventData.delta.normalized.x;

        test?.Rotate(delta);
        player?.SetXInput(delta, SaveDragPos);
        //Debug.Log($"delta {delta}");

        currentEventData = eventData;
    }

    PointerEventData currentEventData;

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        //player?.SetXInput(delta);
        //startPress = eventData.position;
        delta = 0;
    }

    private void SaveDragPos()
    {
        startPress = currentEventData.position;
        delta = 0;
    }

    private Touch theTouch; private Vector2 touchStartPosition, touchEndPosition; private string direction;

    private void Update()
    {
        //var delta = Input.GetAxis("Mouse X");

#if UNITY_EDITOR
        float x = Input.GetAxis("Mouse X");
#elif UNITY_ANDROID

                float x = Input.touches[0].deltaPosition.x;
#endif
        //transform.rotation *= Quaternion.AngleAxis(x * speedRotation, Vector3.up);


        //if (Input.touchCount > 0)
        //{
        //    theTouch = Input.GetTouch(0);

        //    if (theTouch.phase == TouchPhase.Began)
        //    {
        //        touchStartPosition = theTouch.position;



        //    }

        //    else if (theTouch.phase == TouchPhase.Moved || theTouch.phase == TouchPhase.Ended)
        //    {
        //        touchEndPosition = theTouch.position;
        //        float x = touchEndPosition.x - touchStartPosition.x;


        //        test?.Rotate(x);

        //        float y = touchEndPosition.y - touchStartPosition.y;
        //        if (Mathf.Abs(x) == 0 && Mathf.Abs(y) == 0)
        //        { /*direction = “Tapped”;*/ }
        //        else if (Mathf.Abs(x) > Mathf.Abs(y))
        //        { /*direction = x > 0 ? “Right” : “Left”; */}
        //        else { /*direction = y > 0 ? “Up” : “Down”;*/ }
        //    }
        //}

        //test?.Rotate(x);
        //player?.SetXInput(delta);


    }
}
