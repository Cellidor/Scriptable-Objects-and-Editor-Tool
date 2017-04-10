using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Race", menuName = "Data/Race", order = 1)]
public class RaceData : ScriptableObject {

    public List<TimedWaypoint> timedWaypoints;
    public int bulletDamage;
    public float boostSeconds;
    public float boostRecharge;
    public Sprite image;

    public void AddNew()
    {
        //Add a new index position to the end of our list
        timedWaypoints.Add(new TimedWaypoint());
    }

    public void AddSpecific(int index)
    {
        timedWaypoints.Insert(index, new TimedWaypoint());
    }

   public void Remove(int index)
    {
        //Remove an index position from our list at a point in our list array
        timedWaypoints.RemoveAt(index);
    }
}
