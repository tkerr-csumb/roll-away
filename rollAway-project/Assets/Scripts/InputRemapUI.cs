using UnityEngine;
using UnityEngine.UI;

public class InputRemapUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject remapPanel;

    [Header("Buttons")]
    [SerializeField] private Button openRemapButton;
    [SerializeField] private Button closeRemapButton;
    [SerializeField] private Button resetAllButton;

    private void OnEnable()
    {
        if (openRemapButton  != null) openRemapButton.onClick.AddListener(OpenPanel);
        if (closeRemapButton != null) closeRemapButton.onClick.AddListener(ClosePanel);
        if (resetAllButton   != null) resetAllButton.onClick.AddListener(ResetAll);
    }

    private void OnDisable()
    {
        if (openRemapButton  != null) openRemapButton.onClick.RemoveListener(OpenPanel);
        if (closeRemapButton != null) closeRemapButton.onClick.RemoveListener(ClosePanel);
        if (resetAllButton   != null) resetAllButton.onClick.RemoveListener(ResetAll);
    }

    private void OpenPanel()
    {
        if (remapPanel != null) remapPanel.SetActive(true);
    }

    private void ClosePanel()
    {
        if (remapPanel != null) remapPanel.SetActive(false);
    }

    private void ResetAll()
    {
        InputRemapManager.Instance?.ResetAllBindings();
    }
}
