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

        InputRemapManager.OnBindingsChanged += RefreshLabel;
        RefreshLabel();
    }

    private void OnDisable()
    {
        if (rebindButton != null) rebindButton.onClick.RemoveListener(StartRebind);
        if (resetButton  != null) resetButton.onClick.RemoveListener(ResetBinding);

        InputRemapManager.OnBindingsChanged -= RefreshLabel;
        _currentOp?.Cancel();
    }

    private void StartRebind()
    {
        if (InputRemapManager.Instance == null) return;

        SetListeningState(true);

        _currentOp = InputRemapManager.Instance.StartRebind(
            actionName,
            bindingIndex,
            success => SetListeningState(false));
    }

    private void ResetBinding()
    {
        InputRemapManager.Instance?.ResetBinding(actionName, bindingIndex);
    }

    private void RefreshLabel()
    {
        if (bindingLabel == null || InputRemapManager.Instance == null) return;
        bindingLabel.text = InputRemapManager.Instance
            .GetBindingDisplayString(actionName, bindingIndex);
    }

    private void SetListeningState(bool listening)
    {
        if (rebindButton != null) rebindButton.interactable = !listening;
        if (resetButton  != null) resetButton.interactable  = !listening;
        if (bindingLabel != null) bindingLabel.text = listening
            ? waitingText
            : InputRemapManager.Instance?.GetBindingDisplayString(actionName, bindingIndex);
    }
}
