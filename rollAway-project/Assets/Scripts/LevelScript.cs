using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelScript : MonoBehaviour
{
    public LevelDatabase levelDb;
    public Image previewDisplay;
    public TextMeshProUGUI nameDisplay;
    
    private int currentLevelIndex = 0;
    [SerializeField] TMP_Text bestTimeText;
    [SerializeField] TMP_Text bestPickupsText;


    void Start()
    {
        UpdateDisplay();
    }

    public void NextLevel()
    {
        currentLevelIndex = (currentLevelIndex + 1) % levelDb.allLevels.Length;
        UpdateDisplay();
    }

    public void PreviousLevel()
    {
        currentLevelIndex--;
        if (currentLevelIndex < 0) currentLevelIndex = levelDb.allLevels.Length - 1;
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        var level = levelDb.allLevels[currentLevelIndex];
        previewDisplay.sprite = level.previewImage;
        nameDisplay.text = level.displayName;
        string timeKey = "BestTime_" + level.sceneName;
        float best = PlayerPrefs.GetFloat(timeKey, -1f);
        if (best < 0)
            bestTimeText.text = "Best: --:--";
        else
        {
            int minutes = (int)(best / 60f);
            int seconds = (int)(best % 60f);
            bestTimeText.text = string.Format("Best: {0:00}:{1:00}", minutes, seconds);
        }
        string pickupKey = "BestPickups_" + level.sceneName;
        string totalKey = "TotalPickups_" + level.sceneName;
        int bestPickups = PlayerPrefs.GetInt(pickupKey, -1);
        int totalPickups = PlayerPrefs.GetInt(totalKey, 0);
        bestPickupsText.text = bestPickups < 0 ? "Pickups: -/-" : $"{bestPickups}/{totalPickups}";
    }

    public void PlayLevel()
    {
        string sceneToLoad = levelDb.allLevels[currentLevelIndex].sceneName;
        
        if (SceneTransition.Instance != null)
        {
            SceneTransition.Instance.LoadScene(sceneToLoad);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
        }
    }

    public void BackToMenu()
{
    if (SceneTransition.Instance != null)
    {
        SceneTransition.Instance.LoadScene("MainMenu");
    }
    else
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
}