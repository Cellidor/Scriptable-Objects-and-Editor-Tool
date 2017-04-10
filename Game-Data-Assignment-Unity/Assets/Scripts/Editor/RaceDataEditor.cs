using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(RaceData))]
public class RaceDataEditor : Editor {

    public int addWaypoint;
    public int removeWaypoint;
    public Sprite sprite;
    SerializedProperty waypointsList;

    public override void OnInspectorGUI()
    {
        RaceData dataScript = (RaceData)target;
        waypointsList = serializedObject.FindProperty("timedWaypoints");
  //      sprite = (Sprite)EditorGUILayout.ObjectField(new GUIContent("Sprite", "Thumbnail for race."), dataScript.image, typeof(Texture2D), true);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(new GUIContent("Bullet Damage","What % damage each bullet deals"));
        dataScript.bulletDamage = EditorGUILayout.IntSlider(dataScript.bulletDamage, 1,100);

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        dataScript.boostSeconds = EditorGUILayout.FloatField(new GUIContent("Seconds of Boost", "How long the player can boost while holding down the right mouse button"), dataScript.boostSeconds);
        EditorGUILayout.LabelField(new GUIContent("Boost Recharge Time", "how many seconds per second are replenished"));
        dataScript.boostRecharge = EditorGUILayout.Slider(dataScript.boostRecharge,0,1);

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        if(GUILayout.Button(new GUIContent("Add waypoint", "Adds a waypoint ot the end of the list")))
        {
            dataScript.AddNew();
            dataScript.timedWaypoints[dataScript.timedWaypoints.Count-1].waypointNumber = "Waypoint # " + dataScript.timedWaypoints.Count;
            serializedObject.Update();
        }
        if (GUILayout.Button(new GUIContent("Remove waypoint", "Removes the last waypoint on the list")))
        {
            dataScript.Remove(dataScript.timedWaypoints.Count-1);
            serializedObject.Update();
        }
        EditorGUILayout.Space();


        if (GUILayout.Button(new GUIContent("Add specific waypoint", "E.g. Input '2' to insert a waypoint between waypoint's 1 and 2")))
        {
            dataScript.AddSpecific(addWaypoint -1);
            for (int i = 0; i < dataScript.timedWaypoints.Count; i++)
            {
                dataScript.timedWaypoints[i].waypointNumber = "Waypoint # " + (i+1);
            }
            serializedObject.Update();
        }
        addWaypoint = EditorGUILayout.IntField(new GUIContent("Waypoint to Add", "Input the # of the waypoint"), addWaypoint);
        EditorGUILayout.Space();


        if (GUILayout.Button(new GUIContent("Remove specific waypoint", "E.g. Input '2' to remove waypoint #2")))
        {
            dataScript.Remove(removeWaypoint -1);
            for (int i = 0; i < dataScript.timedWaypoints.Count; i++)
            {
                dataScript.timedWaypoints[i].waypointNumber = "Waypoint # " + (i + 1);
            }
            serializedObject.Update();
        }
        removeWaypoint = EditorGUILayout.IntField(new GUIContent("Waypoint to Remove", "Input the # of the waypoint"), removeWaypoint);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("timedWaypoints"), new GUIContent("Waypoint List", "Includes X and Y coordinates, along with the time in seconds to reach each waypoint"), true );

        serializedObject.ApplyModifiedProperties();
        serializedObject.UpdateIfDirtyOrScript();
    }

    
}
