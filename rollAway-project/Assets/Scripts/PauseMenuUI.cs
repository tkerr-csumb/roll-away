using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    private void OnEnable()
    {
        if (resumeButton   != null) resumeButton.onClick.AddListener(OnResume);
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(OnMainMenu);
        if (quitButton     != null) quitButton.onClick.AddListener(OnQuit);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }

    private void OnDisable()
    {
        if (resumeButton   != null) resumeButton.onClick.RemoveListener(OnResume);
        if (mainMenuButton != null) mainMenuButton.onClick.RemoveListener(OnMainMenu);
        if (quitButton     != null) quitButton.onClick.RemoveListener(OnQuit);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    private void OnResume()
    {
        FindFirstObjectByType<PauseManager>()?.Resume();
    }

    private void OnMainMenu()
    {
        FindFirstObjectByType<PauseManager>()?.Resume();
        SceneTransition.Instance?.LoadScene("MainMenu");
    }

    private void OnQuit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}