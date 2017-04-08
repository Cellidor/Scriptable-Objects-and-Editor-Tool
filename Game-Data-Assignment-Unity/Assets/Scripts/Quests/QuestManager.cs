using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour {

    public enum QuestState {
        InProgress,
        Failed,
        Completed
    }

    public QuestState questState;

    public Race raceQuest;
    public int raceWaypointIndex;
    public GameObject waypointMarkerPrefab;

    public Text countdownText;
    public float timeRemaining;
    public Animator statusScreenAnimator;

    public AudioClip questCompleteAudioClip;
    public AudioClip questFailedAudioClip;

    AudioSource audioSource;

    void Awake() {
        audioSource = GetComponent<AudioSource>();
        questState = QuestState.InProgress;

        raceWaypointIndex = 0;
        CreateTimedWaypoint(raceWaypointIndex);
    }

    void Update() {
        if (questState != QuestState.InProgress) return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0) {
            QuestFailed();
            return;
        }

        countdownText.text = string.Format("{0:00}", Mathf.Ceil(timeRemaining));
    }

    void CreateTimedWaypoint(int index) {
        // update countdown
        timeRemaining = raceQuest.timedWaypoints[index].timelimit;

        // place waypoint
        Vector3 position = new Vector3(raceQuest.timedWaypoints[index].x, raceQuest.timedWaypoints[raceWaypointIndex].y, 0);
        GameObject waypointMarker = Instantiate(waypointMarkerPrefab, position, Quaternion.identity) as GameObject;
        waypointMarker.transform.parent = transform;
        waypointMarker.GetComponent<WaypointMarker>().Init(this);
    }

    void QuestFailed() {
        questState = QuestState.Failed;
        statusScreenAnimator.SetTrigger("Quest Failed");
        audioSource.PlayOneShot(questFailedAudioClip);
    }

    void QuestComplete() {
        questState = QuestState.Completed;
        statusScreenAnimator.SetTrigger("Quest Completed");
        audioSource.PlayOneShot(questCompleteAudioClip);
    }


    void OnWaypointReached() {

        if (questState != QuestState.InProgress) return;

        print("Reached waypoint " + raceWaypointIndex + ".");

        if (raceWaypointIndex == raceQuest.timedWaypoints.Count - 1) {
            QuestComplete();
            return;
        }
        raceWaypointIndex++;
        CreateTimedWaypoint(raceWaypointIndex);
    }
}
