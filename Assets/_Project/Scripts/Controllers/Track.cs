using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Dreamteck.Splines;
using Technoprosper.Gameplay.Player;

public class Track : MonoBehaviour
{
    public Transform startPoint;

    private SplineProjector _splineProjector;
    public List<TrackPart> trackParts;

    public Vector3 offset;

    private int _trackPartIndex = 0;
    private GameObject _currentTrack;

    //---------------------------------------------------------------

    public void OnPartEndReached()
    {
        _trackPartIndex++;
    }

    public TrackPart GetNextPart()
    {
        var index = Mathf.Clamp(_trackPartIndex, 0, trackParts.Count - 1);

        return trackParts[index];
    }

    public TrackPart GetNextPartDebug()
    {
        var currentSpline = _splineProjector.spline;
        var currentTrackPart = trackParts.FirstOrDefault(cc => cc.spline == currentSpline);
        var currentTrackPartIndex = trackParts.IndexOf(currentTrackPart);
        var nextIndex = currentTrackPartIndex + 1;

        if (nextIndex > trackParts.Count - 1)
        {
            nextIndex = 0;
        }

        return trackParts[nextIndex];
    }

    public SplineSample GetProjectionPosition(Vector3 worldPosition)
    {
        SplineSample resSplinePoint = new SplineSample();
        _splineProjector.Project(worldPosition, ref resSplinePoint);

        return resSplinePoint;
    }

    //--------------------------------------------------------------- 

    private void Update()
    {
        CheckTrackPart();
    }

    private void CheckTrackPart()
    {
        if (_splineProjector?.result.percent >= .9999999d)
        {
            OnPartEndReached();

            var nextTrackPart = GetNextPartDebug();

            if (nextTrackPart.spline == _splineProjector.spline)
            {
                return;
            }

            _splineProjector.spline = nextTrackPart.spline;
            _splineProjector.RebuildImmediate();
            _splineProjector.SetPercent(0d, false, false);

            float distance = _splineProjector.CalculateLength(0.0, _splineProjector.result.percent); //Get the excess distance after looping            
            _splineProjector.SetDistance(distance); //Set the excess distance along the new spline

            _splineProjector.GetComponent<PlayerGravity>().SetGravity(nextTrackPart.gravity);
        }
    }

    public IEnumerator Init(SplineProjector splineProjector, int splineNumber)
    {
        _splineProjector = splineProjector;

        //_splineProjector.spline = trackParts[_trackPartIndex].spline;
        _splineProjector.spline = trackParts[splineNumber].spline;
        _splineProjector.RebuildImmediate();

        var playerPos = _splineProjector.GetComponent<Player>().transform.position;
        var splineSample = GetProjectionPosition(playerPos);
        _splineProjector.GetComponent<Player>().transform.position = splineSample.position + offset;

        _splineProjector.SetPercent(splineSample.percent, false, false);
        float distance = _splineProjector.CalculateLength(0.0d, _splineProjector.result.percent); //Get the excess distance after looping            
        _splineProjector.SetDistance(distance); //Set the excess distance along the new spline


        yield return null;
    }
}

[Serializable]
public class TrackPart
{
    public SplineComputer spline;
    public float gravity;

}
