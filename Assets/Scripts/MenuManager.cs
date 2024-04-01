using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public TextMeshProUGUI highScoreText;
    public RectTransform backgroundImage;
    [SerializeField] private float backgroundSpeed;
    [SerializeField] private float backgroundMinY;
    [SerializeField] private float backgroundMaxY;
    private int backgroundDirection;
    private void Start()
    {
        backgroundDirection = 1;
        LoadPlayerData();
    }

    private void Update()
    {
        backgroundImage.transform.Translate(Vector3.up * backgroundDirection * Time.deltaTime * backgroundSpeed);

        MoveBackground();
    }
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    private void MoveBackground()
    {
        if((backgroundImage.transform.position.y < backgroundMinY) || (backgroundImage.transform.position.y > backgroundMaxY))
        {
            backgroundDirection = -backgroundDirection;
        }
    }

    private void LoadPlayerData()
    {
        string path = Application.persistentDataPath + "/savedata.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);
            PlayerData.instance.playerScore = saveData.playerScore;
            highScoreText.text = "High Score: " + saveData.playerScore;
        }
    }
}
