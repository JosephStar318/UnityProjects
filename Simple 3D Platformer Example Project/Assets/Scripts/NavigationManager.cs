using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationManager
{
    private Dictionary<string, Vector3> wayPoints = new Dictionary<string, Vector3>();
    public void AddWayPoint(string wayPointName, Vector3 location)
    {
        wayPoints.Add(wayPointName, location);
    }
    public void RemoveWayPoint(string wayPointName, Vector3 location)
    {
        wayPoints.Remove(wayPointName);
    }
    public Vector3 GetWaypointLocation(string wayPointName)
    {
        if(wayPoints.ContainsKey(wayPointName))
        {
            return wayPoints[wayPointName];
        }
        return Vector3.positiveInfinity;
    }
    public bool ChangeWaypointLocation(string wayPointName, Vector3 location)
    {
        if(wayPoints.ContainsKey(wayPointName))
        {
            wayPoints[wayPointName] = location;
            return true;
        }
        else
        {
            return false;
        }
    }
    public void Teleport(GameObject obj,string wayPointName)
    {
        obj.transform.position = wayPoints[wayPointName];
    }

}
