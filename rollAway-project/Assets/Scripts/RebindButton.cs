using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class RebindButton : MonoBehaviour
{
    [Header("Target Binding")]
    [SerializeField] private string actionName;
    [SerializeField] private int    bindingIndex;

    [Header("UI References")]
    [SerializeField] private Button   rebindButton;
    [SerializeField] private TMP_Text bindingLabel;
    [SerializeField] private Button   resetButton;

    [Header("Listening Text")]
    [SerializeField] private string waitingText = "[ press a key ]";

    private InputActionRebindingExtensions.RebindingOperation _currentOp;

    private void OnEnable()
    {
        if (rebindButton != null) rebindButton.onClick.AddListener(StartRebind);
        if (resetButton  != null) resetButton.onClick.AddListener(ResetBinding);

        RollawayInputRemapManager.OnBindingsChanged += RefreshLabel;
        RefreshLabel();
    }

    private void OnDisable()
    {
        if (rebindButton != null) rebindButton.onClick.RemoveListener(StartRebind);
        if (resetButton  != null) resetButton.onClick.RemoveListener(ResetBinding);

        RollawayInputRemapManager.OnBindingsChanged -= RefreshLabel;
        _currentOp?.Cancel();
    }

    private void StartRebind()
    {
        if (RollawayInputRemapManager.Instance == null) return;

        SetListeningState(true);

        _currentOp = RollawayInputRemapManager.Instance.StartRebind(
            actionName,
            bindingIndex,
            success => SetListeningState(false));
    }

    private void ResetBinding()
    {
        RollawayInputRemapManager.Instance?.ResetBinding(actionName, bindingIndex);
    }

    private void RefreshLabel()
    {
        if (bindingLabel == null || RollawayInputRemapManager.Instance == null) return;
        bindingLabel.text = RollawayInputRemapManager.Instance
            .GetBindingDisplayString(actionName, bindingIndex);
    }

    private void SetListeningState(bool listening)
    {
        if (rebindButton != null) rebindButton.interactable = !listening;
        if (resetButton  != null) resetButton.interactable  = !listening;
        if (bindingLabel != null) bindingLabel.text = listening
            ? waitingText
            : RollawayInputRemapManager.Instance?.GetBindingDisplayString(actionName, bindingIndex);
    }
}
