using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelScript : MonoBehaviour
{
    public LevelDatabase levelDb;
    public Image previewDisplay;
    public TextMeshProUGUI nameDisplay;
    
    private int currentLevelIndex = 0;

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