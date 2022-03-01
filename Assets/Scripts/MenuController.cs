using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

// Sets the script to be executed later than all default scripts
// This is helpful for UI, since other things may need to be initialized before setting the UI
[DefaultExecutionOrder(1000)]
public class MenuController : MonoBehaviour
{
    [System.Serializable]
    public class GameData
    {
        public string LastPlayerName;
        public string HighScoreName;
        public int HighScore;
    }

    public GameData GamePersistanceData;
    public static MenuController Instance;

    public GameObject rootCanvas;
    public TMP_InputField inputField;
    public Text highScoreText;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            Destroy(rootCanvas);
            Instance.rootCanvas.SetActive(true);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(rootCanvas);
        
        LoadGameData();
        
    }

   

    // Start is called before the first frame update
    void Start()
    {
        if (GamePersistanceData != null)
        {
            Instance.inputField.text = GamePersistanceData.LastPlayerName;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void goToMainScene()
    {
        rootCanvas.SetActive(false);
        SceneManager.LoadScene(1);
    }

    public void quitGame()
    {
        SaveName(inputField.text);
        #if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
        #else
            Application.Quit(); // original code to quit Unity player
        #endif
    }

   

    public void SaveName(string name)
    {
        GameData data = new GameData();
        data.LastPlayerName = name;
        data.HighScoreName = Instance.GamePersistanceData.HighScoreName;
        data.HighScore = Instance.GamePersistanceData.HighScore;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/gameData.json", json);
    }

    public void CheckHighScore(int score)
    {
        if (score >= Instance.GamePersistanceData.HighScore)
        {
            SaveHighScore(score);
        }
    }


    public void SaveHighScore(int score)
    {
        GameData data = new GameData();
        data.LastPlayerName = Instance.inputField.text;
        data.HighScoreName = Instance.inputField.text;
        data.HighScore = score;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/gameData.json", json);
        if(highScoreText == null) { return;  }
        if (data.HighScore > 0)
        {
            highScoreText.text = "BEST SCORE : " + data.HighScore + " from " + data.HighScoreName;
        } else
        {
            highScoreText.text = "";
        }

    }

    public void LoadGameData()
    {
        string path = Application.persistentDataPath + "/gameData.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            GameData data = JsonUtility.FromJson<GameData>(json);

            Instance.GamePersistanceData = data;
            if (highScoreText == null) { return; }
            if (data.HighScore > 0)
            {
                highScoreText.text = "BEST SCORE : " + data.HighScore + " from " + data.HighScoreName;
            }
            else
            {
                highScoreText.text = "";
            }
        }
    }
}
