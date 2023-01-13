using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Dreamteck.Splines;

public class TrackController : MonoBehaviour
{
    public TrackPart[] trackParts;

    private int _trackPartIndex = 0;

    public void OnPartEndReached()
    {
        _trackPartIndex++;
    }

    public TrackPart GetCurrentPart()
    {
        var index = Mathf.Clamp(_trackPartIndex, 0, trackParts.Length - 1);

        return trackParts[index];
    }
}

[Serializable]
public class TrackPart
{
    public SplineComputer spline;
    public float gravity;

}
