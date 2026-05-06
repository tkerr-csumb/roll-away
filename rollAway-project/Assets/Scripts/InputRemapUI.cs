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
        UIAudio.Instance?.PlayClick();
        if (remapPanel != null) remapPanel.SetActive(true);
    }

    private void ClosePanel()
    {
        UIAudio.Instance?.PlayClick();
        if (remapPanel != null) remapPanel.SetActive(false);
    }

    private void ResetAll()
    {
        UIAudio.Instance?.PlayClick();
        RollawayInputRemapManager.Instance?.ResetAllBindings();
    }
}
