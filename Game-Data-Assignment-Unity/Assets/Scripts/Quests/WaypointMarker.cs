using UnityEngine;
using System.Collections;

public class WaypointMarker : MonoBehaviour {

    public QuestManager questManager;

    public void Init(QuestManager questManager) {
        this.questManager = questManager;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            questManager.SendMessage("OnWaypointReached");
            Destroy(gameObject);
        }
    }
}
