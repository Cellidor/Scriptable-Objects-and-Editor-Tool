using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour {

    public enum QuestState {
        // State of current quest
        InProgress,
        Failed,
        Completed
    }

    public QuestState questState;

    // raceQuest that's made of a list of "Timed Waypoints" which are themselves each an x,y, and time variable.
    public RaceData raceQuest;
    // Keeps track fo which waypoint we're at.
    public int raceWaypointIndex;
    // game object used to represtn a waypoint
    public GameObject waypointMarkerPrefab;

    // text showing how long player has to reach next point.
    public Text countdownText;
    // float for how long player has to reach next point.
    public float timeRemaining;
    // Animator to handle visuals on screen when quest has succeeded/failed.
    public Animator statusScreenAnimator;

    // Audio clips for successful or failed quest.
    public AudioClip questCompleteAudioClip;
    public AudioClip questFailedAudioClip;

    AudioSource audioSource;

    public bool selected;

    void Awake() {
        // Start the race on awake, set to in progress, and spawn the first point.
        audioSource = GetComponent<AudioSource>();
        questState = QuestState.InProgress;

        raceWaypointIndex = 0;
        CreateTimedWaypoint(raceWaypointIndex);
    }

    void Update() {
        // As long as the quest is in progress, count down the time. If time is less than zero, set state to failed.
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
        // Set state to "failed" and proceed with needed animation and audio triggers.
        questState = QuestState.Failed;
        statusScreenAnimator.SetTrigger("Quest Failed");
        audioSource.PlayOneShot(questFailedAudioClip);
    }

    void QuestComplete() {
        // Set state to "completed" and proceed with needed animation and audio triggers.
        questState = QuestState.Completed;
        statusScreenAnimator.SetTrigger("Quest Completed");
        audioSource.PlayOneShot(questCompleteAudioClip);
    }


    void OnWaypointReached() {
        // Ignore if race already failed/won. increment race index, check if player has succeeded, create next waypoint.
        if (questState != QuestState.InProgress) return;

        print("Reached waypoint " + raceWaypointIndex + ".");

        if (raceWaypointIndex == raceQuest.timedWaypoints.Count - 1) {
            QuestComplete();
            return;
        }
        raceWaypointIndex++;
        CreateTimedWaypoint(raceWaypointIndex);
    }

    public void OnDrawGizmosSelected()
    {
        if (selected)
        {
            for (int i = 0; i < raceQuest.timedWaypoints.Count; i++)
            {
                if (i < raceWaypointIndex)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawSphere(new Vector3(raceQuest.timedWaypoints[i].x, raceQuest.timedWaypoints[i].y, 0), 0.3f);
                }
                if (i == raceWaypointIndex)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(new Vector3(raceQuest.timedWaypoints[i].x, raceQuest.timedWaypoints[i].y, 0), 0.3f);
                }
                if (i > raceWaypointIndex)
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawSphere(new Vector3(raceQuest.timedWaypoints[i].x, raceQuest.timedWaypoints[i].y, 0), 0.3f);
                }
            }
        }

    }

}
