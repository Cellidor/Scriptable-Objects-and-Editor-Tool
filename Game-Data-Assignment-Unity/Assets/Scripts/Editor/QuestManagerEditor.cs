using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(QuestManager))]
public class QuestManagerEditor : Editor
{

    public void OnSceneGUI()
    {
        // get our current selection
        Object[] selection = Selection.GetFiltered(typeof(QuestManager), SelectionMode.Deep);
        QuestManager questManager = null;
        if (selection.Length > 0)
        {
            questManager = selection[0] as QuestManager;
        }
        if (null == questManager)
        {
            return;
        }

        Handles.BeginGUI();

        // get scene view reference
        SceneView view = SceneView.currentDrawingSceneView;

        // render image
        //   Vector2 size = new Vector2(questManager.raceQuest.image.texture.width, questManager.raceQuest.image.texture.height);

        Vector3 spawnPoint = Vector3.one; ;
        //  Vector3 spawnPoint = questManager.SpawnPosition;
        //Vector3 texturePosition = view.camera.WorldToScreenPoint(spawnPoint);

        float screenHeightInPixels = view.camera.pixelHeight;

       // texturePosition.y = screenHeightInPixels - texturePosition.y;
        //texturePosition = texturePosition - new Vector3(questManager.raceQuest.image.texture.width / 2, questManager.raceQuest.image.texture.height / 2);

       // Rect textureRect = new Rect(texturePosition, size);

       // GUI.DrawTexture(textureRect, questManager.raceQuest.image.texture);

        // Render text label
        string text = questManager.raceQuest.name + ", " + (questManager.raceQuest.timedWaypoints.Count + 1) + " total waypoints";
        Vector2 textSize = GUI.skin.label.CalcSize(new GUIContent(text));
        Vector2 textPosition = Vector3.zero;
      //  textPosition.y = textPosition.y + textureRect.height;

        Rect textRect = new Rect(textPosition, textSize);
        Color originalColour = GUI.color;
        GUI.Label(textRect, text);

        Handles.EndGUI();

        Matrix4x4 original = Handles.matrix;
        Handles.matrix = questManager.transform.localToWorldMatrix;

        for (int i = 0; i < questManager.raceQuest.timedWaypoints.Count; i++)
        {
            float x = questManager.raceQuest.timedWaypoints[i].x;
            float y = questManager.raceQuest.timedWaypoints[i].y;
            Vector3 xyz = new Vector3(x, y, 0);
            xyz = Handles.PositionHandle(xyz, questManager.transform.rotation);
            questManager.raceQuest.timedWaypoints[i].x = xyz.x;
            questManager.raceQuest.timedWaypoints[i].y = xyz.y;
            EditorUtility.SetDirty(questManager.raceQuest);
        }


        Handles.matrix = original;
    }
}
