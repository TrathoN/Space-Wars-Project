using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public TextMeshProUGUI highScoreText;
    public RectTransform backgroundImage;
    public GameObject settingsMenu;
    [SerializeField] private float backgroundSpeed;
    [SerializeField] private float backgroundMinY;
    [SerializeField] private float backgroundMaxY;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private int backgroundDirection;
    private void Start()
    {
        Time.timeScale = 1.0f;
        LoadPlayerData();
        SetMenuSounds();
    }

    private void Update()
    {
        backgroundImage.transform.Translate(Vector3.up * backgroundDirection * Time.deltaTime * backgroundSpeed);
        SetDirectionBackground();
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

    private void SetMenuSounds()
    {
        PlayerData.instance.musicVolume = musicSlider.value;
        PlayerData.instance.sfxVolume = sfxSlider.value;

        musicSlider.onValueChanged.AddListener((v) =>
        {
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>().volume = v;
            PlayerData.instance.musicVolume = v;
        });

        sfxSlider.onValueChanged.AddListener((v) =>
        {
            PlayerData.instance.sfxVolume = v;
        });
    }

    private void SetDirectionBackground()
    {
        if ((backgroundImage.transform.position.y < backgroundMinY) || (backgroundImage.transform.position.y > backgroundMaxY))
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
