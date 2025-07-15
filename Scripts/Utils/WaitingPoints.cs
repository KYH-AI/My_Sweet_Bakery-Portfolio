using System.Collections.Generic;
using UnityEngine;

public class WaitingPoints : MonoBehaviour
{
    public Transform GetWayPoint(int index)
    {
        return this.transform.GetChild(index);
    }

    public int GetWayPointCount()
    {
        return this.transform.childCount;
    }
    
    public List<Transform> GetAllWayPointsList()
    {
        List<Transform> waypoints = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            waypoints.Add(transform.GetChild(i));
        }
        return waypoints;
    }
}
