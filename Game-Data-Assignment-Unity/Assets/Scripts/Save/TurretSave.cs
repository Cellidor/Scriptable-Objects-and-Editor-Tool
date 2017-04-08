using System;
using UnityEngine;

[Serializable]
public class TurretSave : Save {

    public Data data;
    private Turret turret;
    private string jsonString;

    [Serializable]
    public class Data : BaseData {
        public Vector3 position;
        public Vector3 barrelEulerAngles;
        public float previousFiringTime;
    }

    void Awake() {
        turret = GetComponent<Turret>();
        data = new Data();
    }

    public override string Serialize() {
        data.prefabName = prefabName;
        data.position = turret.transform.position;
        data.barrelEulerAngles = turret.barrel.transform.eulerAngles;
        data.previousFiringTime = turret.previousFiringTime;

        jsonString = JsonUtility.ToJson(data);
        return (jsonString);
    }

    public override void Deserialize(string jsonData) {
        JsonUtility.FromJsonOverwrite(jsonData, data);
        turret.transform.position = data.position;
        turret.barrel.transform.eulerAngles = data.barrelEulerAngles;
        turret.previousFiringTime = data.previousFiringTime;
        turret.transform.parent = GameObject.Find("Turrets").transform;
    }
}
