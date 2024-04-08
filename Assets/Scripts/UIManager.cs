using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public int totalScore;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI pauseText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI menuScoreText;
    public TextMeshProUGUI bossBarText;
    public GameObject pauseButton;
    public GameObject pauseMenu;
    public GameObject bossBarField;
    public RectTransform bossBar;
    public RectTransform progressBar;
    public Slider musicSlider;
    public Slider sfxSlider;

    private bool isPaused;

    private void Start()
    {
        Time.timeScale = 1.0f;
        SetSounds();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameObject.Find("SettingsMenu"))
            {
                GameObject.Find("SettingsMenu").SetActive(false);
            }
            else
            {
                PauseGame();
            }
        }
    }
    public void PauseGame()
    {
        if(!isPaused)
        {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
            menuScoreText.text = "Score: " + totalScore;
            isPaused = !isPaused;
        }
        else
        {
            Time.timeScale = 1.0f;
            pauseMenu.SetActive(false);
            isPaused = !isPaused;
        }
    }

    public void ScoreSum(int score)
    {
        totalScore += score;
        scoreText.text = "Score: " + totalScore;
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        menuScoreText.text = "Score: " + totalScore;
        pauseButton.SetActive(false); 
        pauseText.text = "GAME OVER";
    }

    public void Victory()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        menuScoreText.text = "Score: " + totalScore;
        pauseButton.SetActive(false);
        pauseText.text = "VICTORY";
    }

    public void HealthUpdate(int health)
    {
        if(health < 0)
        {
            health = 0;
        }
        healthText.text = $"{health}";
    }

    public void ExitMenu()
    {
        SavePlayerHighScore();
        SceneManager.LoadScene(0);
    }

    public void RestartGame()
    {
        SavePlayerHighScore();
        SceneManager.LoadScene(1);
    }

    public void SetProgressBar(int progress)
    {
        progressBar.sizeDelta = new Vector2(28.56f, progress * 70);
        progressBar.anchoredPosition = new Vector3(0.4f, (35 * progress) + 5, 0);
    }

    public void SetBossHealthBar(float bossHealth)
    {
        float barSizeX = (bossHealth / 3000) * 700;
        bossBar.sizeDelta = new Vector2(barSizeX, 42.5f);
        bossBar.anchoredPosition = new Vector3(barSizeX / 2, -1);
        bossBarText.text = bossHealth + " / 3000";
    }

    private void SetSounds()
    {
        musicSlider.value = PlayerData.instance.musicVolume;
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>().volume = PlayerData.instance.musicVolume;

        sfxSlider.value = PlayerData.instance.sfxVolume;
        GameObject.Find("SpawnManager").GetComponent<AudioSource>().volume = PlayerData.instance.sfxVolume;

        musicSlider.onValueChanged.AddListener((v) =>
        {
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>().volume = v;
            PlayerData.instance.musicVolume = v;
        });

        sfxSlider.onValueChanged.AddListener((v) =>
        {
            GameObject.Find("SpawnManager").GetComponent<AudioSource>().volume = v;
            PlayerData.instance.sfxVolume = v;
        });
    }

    private void SavePlayerHighScore()
    {
        string path = Application.persistentDataPath + "/savedata.json";
        if (totalScore > PlayerData.instance.playerScore)
        {
            PlayerData.instance.playerScore = totalScore;
            SaveData data = new SaveData();
            data.playerScore = totalScore;
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(path, json);
        }
    }
}
