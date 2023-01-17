using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Dreamteck.Splines;

public class TrackController : MonoBehaviour, IController
{
    public GameObject trackPrefab;
    
    public SplineProjector splineProjector;
    public List<TrackPart> trackParts;

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
        var currentSpline = splineProjector.spline;
        var currentTrackPart = trackParts.FirstOrDefault(cc => cc.spline == currentSpline);
        var currentTrackPartIndex = trackParts.IndexOf(currentTrackPart);
        var nextIndex = currentTrackPartIndex + 1;

        if (nextIndex > trackParts.Count - 1)
        {
            nextIndex = 0;
        }

        return trackParts[nextIndex];
    }

    //---------------------------------------------------------------
    
    //TODO: Store every level in separate Scene
    private void SpawnTrack()
    {
        _currentTrack = Instantiate(trackPrefab);
    }


    private void SetCurrentSpline()
    {

    }

    private void Start()
    {
        //var nextTrackPart = GetNextPart();
        //splineProjector.spline = nextTrackPart.spline;
        //splineProjector.RebuildImmediate();

        //splineProjector.GetComponent<PlayerGravity>().SetGravity(nextTrackPart.gravity);

    }

    private void Update()
    {
        CheckTrackPart();
    }

    private void CheckTrackPart()
    {
        if (splineProjector?.result.percent >= .9999999d)
        {
            OnPartEndReached();

            var nextTrackPart = GetNextPartDebug();

            if (nextTrackPart.spline == splineProjector.spline)
            {
                return;
            }

            splineProjector.spline = nextTrackPart.spline;
            splineProjector.RebuildImmediate();
            splineProjector.SetPercent(0d, false, false);

            float distance = splineProjector.CalculateLength(0.0, splineProjector.result.percent); //Get the excess distance after looping            
            splineProjector.SetDistance(distance); //Set the excess distance along the new spline

            splineProjector.GetComponent<PlayerGravity>().SetGravity(nextTrackPart.gravity);
        }
    }

    public IEnumerator Init()
    {
        yield return null;
    }
}

[Serializable]
public class TrackPart
{
    public SplineComputer spline;
    public float gravity;

}
