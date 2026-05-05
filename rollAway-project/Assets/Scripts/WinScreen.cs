using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{
    [Header("Stats Display")]
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI collectiblesText;
    [SerializeField] private TextMeshProUGUI parTimeText;

    [Header("Buttons")]
    [SerializeField] private Button replayButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button nextLevelButton;

    [Header("Level Database")]
    [SerializeField] private LevelDatabase levelDb;

    private string currentSceneName;
    [Header("Header")]
    [SerializeField] private TextMeshProUGUI headerText;
    public static bool IsShowing { get; private set; }

    private void OnEnable()
    {
        if (replayButton   != null) replayButton.onClick.AddListener(OnReplay);
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(OnMainMenu);
        if (nextLevelButton != null) nextLevelButton.onClick.AddListener(OnNextLevel);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }

    private void OnDisable()
    {
        IsShowing = false;
        if (replayButton   != null) replayButton.onClick.RemoveListener(OnReplay);
        if (mainMenuButton != null) mainMenuButton.onClick.RemoveListener(OnMainMenu);
        if (nextLevelButton != null) nextLevelButton.onClick.RemoveListener(OnNextLevel);
    }

    public void Show(float time, int pickups, int total)
    {
        IsShowing = true;
        if (headerText != null) headerText.gameObject.SetActive(true);
        currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        gameObject.SetActive(true);

        int minutes = (int)(time / 60f);
        int seconds = (int)(time % 60f);
        if (timeText != null)
            timeText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);

        if (collectiblesText != null)
            collectiblesText.text = $"Collectibles: {pickups}/{total}";

        if (parTimeText != null)
        {
            float par = GetParTime(currentSceneName);
            if (par > 0f)
            {
                int pMin = (int)(par / 60f);
                int pSec = (int)(par % 60f);
                string parLabel = string.Format("{0:00}:{1:00}", pMin, pSec);
                parTimeText.text = time <= par
                    ? $"Par: {parLabel}"
                    : $"Par: {parLabel}";
            }
            else
            {
                parTimeText.gameObject.SetActive(false);
            }
        }

        if (nextLevelButton != null)
            nextLevelButton.gameObject.SetActive(HasNextLevel(currentSceneName));
    }

    private void OnReplay()
    {
        Time.timeScale = 2f;
        SceneTransition.Instance?.LoadScene(currentSceneName);
    }

    private void OnMainMenu()
    {
        Time.timeScale = 2f;
        SceneTransition.Instance?.LoadScene("MainMenu");
    }

    private void OnNextLevel()
    {
        string next = GetNextSceneName(currentSceneName);
        if (next != null)
        {
            Time.timeScale = 2f;
            SceneTransition.Instance?.LoadScene(next);
        }
    }

    private float GetParTime(string sceneName)
    {
        if (levelDb == null) return 0f;
        foreach (var level in levelDb.allLevels)
            if (level.sceneName == sceneName)
                return level.parTime;
        return 0f;
    }

    private bool HasNextLevel(string sceneName)
    {
        return GetNextSceneName(sceneName) != null;
    }

    private string GetNextSceneName(string sceneName)
    {
        if (levelDb == null) return null;
        for (int i = 0; i < levelDb.allLevels.Length - 1; i++)
            if (levelDb.allLevels[i].sceneName == sceneName)
                return levelDb.allLevels[i + 1].sceneName;
        return null;
    }
    public void ShowLose()
    {
        IsShowing = true;
        if (headerText != null) headerText.text = "You Died!";
        if (timeText         != null) timeText.gameObject.SetActive(false);
        if (collectiblesText != null) collectiblesText.gameObject.SetActive(false);
        if (parTimeText      != null) parTimeText.gameObject.SetActive(false);
        currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        gameObject.SetActive(true);
        if (nextLevelButton != null)
            nextLevelButton.gameObject.SetActive(HasNextLevel(currentSceneName));
    }
}