using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseAudioUI : MonoBehaviour
{
    [Header("Music Toggle")]
    [SerializeField] private Button    musicButton;
    [SerializeField] private Image     musicIcon;
    [SerializeField] private TMP_Text  musicLabel;
    [SerializeField] private Sprite    musicOnSprite;
    [SerializeField] private Sprite    musicOffSprite;

    [Header("SFX Toggle")]
    [SerializeField] private Button    sfxButton;
    [SerializeField] private Image     sfxIcon;
    [SerializeField] private TMP_Text  sfxLabel;
    [SerializeField] private Sprite    sfxOnSprite;
    [SerializeField] private Sprite    sfxOffSprite;

    [Header("Colors")]
    [SerializeField] private Color activeColor   = Color.white;
    [SerializeField] private Color inactiveColor = new Color(0.4f, 0.4f, 0.4f, 1f);

    private void OnEnable()
    {
        if (musicButton != null) musicButton.onClick.AddListener(OnMusicClicked);
        if (sfxButton   != null) sfxButton.onClick.AddListener(OnSFXClicked);

        AudioManager.OnMusicToggled += RefreshMusic;
        AudioManager.OnSFXToggled   += RefreshSFX;

        RefreshAll();
    }

    private void OnDisable()
    {
        if (musicButton != null) musicButton.onClick.RemoveListener(OnMusicClicked);
        if (sfxButton   != null) sfxButton.onClick.RemoveListener(OnSFXClicked);

        AudioManager.OnMusicToggled -= RefreshMusic;
        AudioManager.OnSFXToggled   -= RefreshSFX;
    }

    private void OnMusicClicked() => AudioManager.Instance?.ToggleMusic();
    private void OnSFXClicked()   => AudioManager.Instance?.ToggleSFX();

    private void RefreshAll()
    {
        if (AudioManager.Instance == null) return;
        RefreshMusic(AudioManager.Instance.MusicEnabled);
        RefreshSFX(AudioManager.Instance.SFXEnabled);
    }

    private void RefreshMusic(bool enabled)
    {
        if (musicIcon  != null) musicIcon.sprite = enabled ? musicOnSprite  : musicOffSprite;
        if (musicIcon  != null) musicIcon.color  = enabled ? activeColor    : inactiveColor;
        if (musicLabel != null) musicLabel.text  = enabled ? "Music: ON"    : "Music: OFF";
    }

    private void RefreshSFX(bool enabled)
    {
        if (sfxIcon  != null) sfxIcon.sprite = enabled ? sfxOnSprite  : sfxOffSprite;
        if (sfxIcon  != null) sfxIcon.color  = enabled ? activeColor  : inactiveColor;
        if (sfxLabel != null) sfxLabel.text  = enabled ? "SFX: ON"    : "SFX: OFF";
    }
}
