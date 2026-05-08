using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public static event System.Action OnPaused;
    public static event System.Action OnResumed;
    public static bool IsPaused { get; private set; }

    [Header("UI")]
    [SerializeField]
    private GameObject pausePanel;

    private InputActions inputActions;

    private void Awake()
    {
        inputActions = new InputActions();
        RollawayInputRemapManager.Instance?.ApplyOverridesTo(inputActions.asset);

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    private void OnEnable()
    {
        RollawayInputRemapManager.OnBindingsChanged += RefreshBindings;
        inputActions.Player.Pause.performed += OnPausePerformed;
        inputActions.Enable();
    }

    private void OnDisable()
    {
        RollawayInputRemapManager.OnBindingsChanged -= RefreshBindings;
        inputActions.Player.Pause.performed -= OnPausePerformed;
        inputActions.Disable();
    }

    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        Toggle();
    }

    public void Toggle()
    {
        if (IsPaused)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        if (IsPaused)
            return;

        IsPaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null)
            pausePanel.SetActive(true);

        OnPaused?.Invoke();
    }

    public void Resume()
    {
        if (!IsPaused)
            return;

        IsPaused = false;
        Time.timeScale = 2f;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        OnResumed?.Invoke();
    }

    private void RefreshBindings()
    {
        RollawayInputRemapManager.Instance?.ApplyOverridesTo(inputActions.asset);
    }
}
