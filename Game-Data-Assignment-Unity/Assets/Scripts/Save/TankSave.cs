using System;
using UnityEngine;

[Serializable]
public class TankSave : Save {

    public Data data;
    private Tank tank;
    private string jsonString;

    [Serializable]
    public class Data : BaseData {
        public Vector3 position;
        public Vector3 eulerAngles;
        public Vector3 destination;
    }

    void Awake() {
        tank = GetComponent<Tank>();
        data = new Data();
    }

    public override string Serialize() {
        data.prefabName = prefabName;
        data.position = tank.transform.position;
        data.eulerAngles = tank.transform.eulerAngles;
        data.destination = tank.destination;
        jsonString = JsonUtility.ToJson(data);
        return (jsonString);
    }

    public override void Deserialize(string jsonData) {
        JsonUtility.FromJsonOverwrite(jsonData, data);
        tank.transform.position = data.position;
        tank.transform.eulerAngles = data.eulerAngles;
        tank.destination = data.destination;
        tank.name = "Tank";
    }
}