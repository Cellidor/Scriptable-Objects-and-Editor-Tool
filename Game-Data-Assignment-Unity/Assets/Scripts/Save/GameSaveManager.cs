using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using System.IO;

[Serializable]
public class GameSaveManager : MonoBehaviour {

    public string gameDataFilename = "game-data.json";
    private string gameDataFilePath;
    public List<string> saveItems;
    public bool firstPlay = true;

    // Singleton pattern from:
    // http://clearcutgames.net/home/?p=437

    // Static singleton property
    public static GameSaveManager Instance { get; private set; }


    void Awake() {
        // First we check if there are any other instances conflicting
        if (Instance != null && Instance != this) {
            // If that is the case, we destroy other instances
            Destroy(gameObject);
            return;
        }


        // Here we save our singleton instance
        Instance = this;

        // Furthermore we make sure that we don't destroy between scenes (this is optional)
        DontDestroyOnLoad(gameObject);

        gameDataFilePath = Application.dataPath + "/" + gameDataFilename;
        saveItems = new List<string>();
    }


    public void AddObject(string item) {
        saveItems.Add(item);
    }

    public void Save() {
        saveItems.Clear();

        Save[] saveableObjects = GameObject.FindObjectsOfType(typeof(Save)) as Save[];
        foreach (Save saveableObject in saveableObjects) {
            saveItems.Add(saveableObject.Serialize());
        }

        using (StreamWriter gameDataFileStream = new StreamWriter(gameDataFilePath)) {
            foreach (string item in saveItems) {
                gameDataFileStream.WriteLine(item);
            }
        }
    }

    public void Load() {
        firstPlay = false;
        saveItems.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }



    void OnLevelWasLoaded(int level) {
        if (firstPlay) return;

        LoadSaveGameData();
        DestroyAllSaveableObjectsInScene();
        CreateGameObjects();
    }

    void LoadSaveGameData() {
        using (StreamReader gameDataFileStream = new StreamReader(gameDataFilePath)) {
            while (gameDataFileStream.Peek() >= 0) {
                string line = gameDataFileStream.ReadLine().Trim();
                if (line.Length > 0) {
                    saveItems.Add(line);
                }
            }
        }
    }

    void DestroyAllSaveableObjectsInScene() {
        Save[] saveableObjects = GameObject.FindObjectsOfType(typeof(Save)) as Save[];
        foreach (Save saveableObject in saveableObjects) {
            Destroy(saveableObject.gameObject);
        }
    }

    void CreateGameObjects() {
        foreach (string saveItem in saveItems) {
            string pattern = @"""prefabName"":""";
            int patternIndex = saveItem.IndexOf(pattern);
            int valueStartIndex = saveItem.IndexOf('"', patternIndex + pattern.Length - 1) + 1;
            int valueEndIndex = saveItem.IndexOf('"', valueStartIndex);
            string prefabName = saveItem.Substring(valueStartIndex, valueEndIndex - valueStartIndex);

            GameObject item = Instantiate(Resources.Load(prefabName)) as GameObject;
            item.SendMessage("Deserialize", saveItem);
        }
    }
}
