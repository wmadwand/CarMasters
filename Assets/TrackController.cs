using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Dreamteck.Splines;

public class TrackController : MonoBehaviour
{
    public SplineProjector splineProjector;
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

    private void Start()
    {
        var nextTrackPart = GetCurrentPart();
        splineProjector.spline = nextTrackPart.spline;
        splineProjector.RebuildImmediate();

        splineProjector.GetComponent<PlayerGravity>().SetGravity(nextTrackPart.gravity);

    }

    private void Update()
    {
        CheckTrackPart();
    }

    private void CheckTrackPart()
    {
        if (splineProjector.result.percent >= 1)
        {
            OnPartEndReached();

            var nextTrackPart = GetCurrentPart();

            if (nextTrackPart.spline == splineProjector.spline)
            {
                return;
            }

            splineProjector.spline = nextTrackPart.spline;
            splineProjector.RebuildImmediate();
            splineProjector.SetPercent(0d, false, false);

            splineProjector.GetComponent<PlayerGravity>().SetGravity(nextTrackPart.gravity);
        }
    }
}

[Serializable]
public class TrackPart
{
    public SplineComputer spline;
    public float gravity;

}
