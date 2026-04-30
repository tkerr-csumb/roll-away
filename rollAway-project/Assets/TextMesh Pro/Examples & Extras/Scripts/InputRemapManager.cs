using UnityEngine;
using UnityEngine.InputSystem;

public class InputRemapManager : MonoBehaviour
{
    public static InputRemapManager Instance { get; private set; }

    public static event System.Action OnBindingsChanged;

    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset actions;

    private const string BindingsPrefKey = "InputBindingOverrides";

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadBindings();
    }

    public InputActionRebindingExtensions.RebindingOperation StartRebind(
        string actionName,
        int bindingIndex,
        System.Action<bool> onComplete)
    {
        InputAction action = actions.FindAction(actionName);
        if (action == null)
        {
            Debug.LogWarning($"[InputRemapManager] Action '{actionName}' not found.");
            onComplete?.Invoke(false);
            return null;
        }

        action.Disable();

        return action
            .PerformInteractiveRebinding(bindingIndex)
            .WithCancelingThrough("<Keyboard>/escape")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(op =>
            {
                op.Dispose();
                action.Enable();
                SaveBindings();
                OnBindingsChanged?.Invoke();
                onComplete?.Invoke(true);
            })
            .OnCancel(op =>
            {
                op.Dispose();
                action.Enable();
                onComplete?.Invoke(false);
            })
            .Start();
    }

    public void ResetBinding(string actionName, int bindingIndex)
    {
        InputAction action = actions.FindAction(actionName);
        if (action == null) return;
        action.RemoveBindingOverride(bindingIndex);
        SaveBindings();
        OnBindingsChanged?.Invoke();
    }

    public void ResetAllBindings()
    {
        foreach (var map in actions.actionMaps)
            map.RemoveAllBindingOverrides();
        PlayerPrefs.DeleteKey(BindingsPrefKey);
        OnBindingsChanged?.Invoke();
    }

    public string GetBindingDisplayString(string actionName, int bindingIndex)
    {
        InputAction action = actions.FindAction(actionName);
        if (action == null) return "?";
        return action.GetBindingDisplayString(bindingIndex);
    }

    private void SaveBindings()
    {
        string json = actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString(BindingsPrefKey, json);
        PlayerPrefs.Save();
    }

    private void LoadBindings()
    {
        if (PlayerPrefs.HasKey(BindingsPrefKey))
            actions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(BindingsPrefKey));
    }
}
