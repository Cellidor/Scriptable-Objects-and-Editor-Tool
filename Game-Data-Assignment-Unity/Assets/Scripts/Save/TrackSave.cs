using System;
using UnityEngine;

[Serializable]
public class TrackSave : Save {

    public Data data;
    private Track track;
    private string jsonString;

    [Serializable]
    public class Data : BaseData {
        public Vector3 position;
        public Vector3 eulerAngles;
        public Color color;
    }

    void Awake() {
        track = GetComponent<Track>();
        data = new Data();
    }

    public override string Serialize() {
        data.prefabName = prefabName;
        data.position = track.transform.position;
        data.eulerAngles = track.transform.eulerAngles;
        data.color = track.trackColor;

        jsonString = JsonUtility.ToJson(data);
        return (jsonString);
    }

    public override void Deserialize(string jsonData) {
        JsonUtility.FromJsonOverwrite(jsonData, data);
        track.transform.position = data.position;
        track.transform.eulerAngles = data.eulerAngles;
        track.trackColor = data.color;
        track.transform.parent = GameObject.Find("Tracks").transform;
        track.name = "Track";
    }
}