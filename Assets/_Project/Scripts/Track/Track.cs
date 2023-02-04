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
    public Vector3 offset;
    public List<TrackPart> trackParts;

    private SplineProjector _splineProjector;
    private int _trackPartIndex = 0;    

    //---------------------------------------------------------------

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

    private void OnPartEndReached()
    {
        //if (_splineProjector?.result.percent >= .9999999d)
        //{
        //var nextTrackPart = GetNextPartDebug();
        //var startPointOfNextPart = nextTrackPart.spline.GetPoint(0);
        ////var sss = nextTrackPart.spline.Evaluate(0d);
        //var playerPos = _splineProjector.transform.position;

        //var direction = startPointOfNextPart.position - _splineProjector.spline.EvaluatePosition(1d);
        //var normal = direction.normalized;
        ////var normal = _splineProjector.result.forward;
        //var plane = new Plane(normal, startPointOfNextPart.position);
        //var side = plane.GetSide(playerPos);



        //DrawPlane(startPointOfNextPart.position, normal);

        //if (!side /*&& offset.y < offsetToCheck.y !!!! */) { return; }

        _trackPartIndex++;
        var nextTrackPart = GetNextPart();

        _splineProjector.spline = nextTrackPart.spline;
        _splineProjector.RebuildImmediate();
        _splineProjector.SetPercent(0d, false, false);

        float distance = _splineProjector.CalculateLength(0.0, _splineProjector.result.percent); //Get the excess distance after looping            
        _splineProjector.SetDistance(distance); //Set the excess distance along the new spline
        _splineProjector.GetComponent<PlayerGravity>().SetGravity(nextTrackPart.gravity);
    }

    private void Awake()
    {
        TrackPartFinishTrigger.OnPlayerEnter += FinishTrackPartZone_OnPlayerEnter;
    }

    private void OnDestroy()
    {
        TrackPartFinishTrigger.OnPlayerEnter -= FinishTrackPartZone_OnPlayerEnter;
    }

    private void FinishTrackPartZone_OnPlayerEnter()
    {
        OnPartEndReached();
    }

    public void DrawPlane(Vector3 position, Vector3 normal)
    {
        Vector3 v3;
        if (normal.normalized != Vector3.forward)
            v3 = Vector3.Cross(normal, Vector3.forward).normalized * normal.magnitude;
        else
            v3 = Vector3.Cross(normal, Vector3.up).normalized * normal.magnitude; ;
        var corner0 = position + v3;
        var corner2 = position - v3;
        var q = Quaternion.AngleAxis(90.0f, normal);
        v3 = q * v3;
        var corner1 = position + v3;
        var corner3 = position - v3;
        Debug.DrawLine(corner0, corner2, Color.green);
        Debug.DrawLine(corner1, corner3, Color.green);
        Debug.DrawLine(corner0, corner1, Color.green);
        Debug.DrawLine(corner1, corner2, Color.green);
        Debug.DrawLine(corner2, corner3, Color.green);
        Debug.DrawLine(corner3, corner0, Color.green);
        Debug.DrawRay(position, normal, Color.red);
    }

    public IEnumerator Init(Player player, int splineNumber = 0)
    {
        _trackPartIndex = splineNumber;
        _splineProjector = player.SplineProjector;

        //_splineProjector.spline = trackParts[_trackPartIndex].spline;
        _splineProjector.spline = trackParts[_trackPartIndex].spline;
        _splineProjector.RebuildImmediate();

        var playerPos = _splineProjector.GetComponent<Player>().transform.position;
        var splineSample = GetProjectionPosition(playerPos);
        player.transform.position = splineSample.position + offset;
        _splineProjector.GetComponent<PlayerGravity>().SetGravity(trackParts[_trackPartIndex].gravity);

        _splineProjector.motion.rotationOffset = Vector3.zero;

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
