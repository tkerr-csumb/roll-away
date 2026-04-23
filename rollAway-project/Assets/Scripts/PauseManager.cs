using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public static event System.Action OnPaused;
    public static event System.Action OnResumed;
    public static bool IsPaused { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject pausePanel;

    [Header("Input (New Input System)")]
    [SerializeField] private InputAction pauseAction = new InputAction(
        "Pause",
        InputActionType.Button,
        "<Keyboard>/escape"
    );

    private void Awake()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    private void OnEnable()
    {
        pauseAction.Enable();
        pauseAction.performed += _ => Toggle();
    }

    private void OnDisable()
    {
        pauseAction.performed -= _ => Toggle();
        pauseAction.Disable();
    }

    public void Toggle()
    {
        if (IsPaused) Resume();
        else          Pause();
    }

    public void Pause()
    {
        if (IsPaused) return;

        IsPaused       = true;
        Time.timeScale = 0f;

        if (pausePanel != null)
            pausePanel.SetActive(true);

        OnPaused?.Invoke();
    }

    public void Resume()
    {
        if (!IsPaused) return;

        IsPaused       = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        OnResumed?.Invoke();
    }
}
