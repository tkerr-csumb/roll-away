using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class RollawayInputRemapManager : MonoBehaviour
{
    public static RollawayInputRemapManager Instance { get; private set; }
    public static event Action OnBindingsChanged;

    [SerializeField] private InputActionAsset inputActions;

    private const string BindingOverridesKey = "InputBindingOverrides";

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
        Action<bool> onComplete)
    {
        InputAction action = inputActions.FindAction(actionName);
        if (action == null) { onComplete?.Invoke(false); return null; }

        action.Disable();

        return action
            .PerformInteractiveRebinding(bindingIndex)
            .WithCancelingThrough("<Keyboard>/escape")
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
        InputAction action = inputActions.FindAction(actionName);
        if (action == null) return;

        action.RemoveBindingOverride(bindingIndex);
        SaveBindings();
        OnBindingsChanged?.Invoke();
    }

    public void ResetAllBindings()
    {
        inputActions.RemoveAllBindingOverrides();
        SaveBindings();
        OnBindingsChanged?.Invoke();
    }

    public string GetBindingDisplayString(string actionName, int bindingIndex)
    {
        InputAction action = inputActions.FindAction(actionName);
        if (action == null) return "?";
        return action.GetBindingDisplayString(bindingIndex);
    }

    private void SaveBindings()
    {
        string json = inputActions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString(BindingOverridesKey, json);
        PlayerPrefs.Save();
    }

    private void LoadBindings()
    {
        if (PlayerPrefs.HasKey(BindingOverridesKey))
        {
            string json = PlayerPrefs.GetString(BindingOverridesKey);
            inputActions.LoadBindingOverridesFromJson(json);
        }
    }
    public void ApplyOverridesTo(InputActionAsset target)
    {
        if (PlayerPrefs.HasKey(BindingOverridesKey))
            target.LoadBindingOverridesFromJson(PlayerPrefs.GetString(BindingOverridesKey));
    }
}