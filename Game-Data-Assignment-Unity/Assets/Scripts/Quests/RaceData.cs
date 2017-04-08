using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Race", menuName = "Data/Race", order = 1)]
public class RaceData : ScriptableObject {

    public List<TimedWaypoint> timedWaypoints;
    public int bulletDamage;
    public float boostSeconds;
    public float boostRecharge;
}
