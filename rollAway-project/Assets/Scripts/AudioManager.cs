using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public static event System.Action<bool> OnMusicToggled;
    public static event System.Action<bool> OnSFXToggled;

    public bool MusicEnabled { get; private set; } = true;
    public bool SFXEnabled   { get; private set; } = true;

    [Header("Mixer")]
    [SerializeField] private AudioMixer mixer;

    [Header("Mixer Exposed Parameter Names")]
    [SerializeField] private string musicParam = "MusicVolume";
    [SerializeField] private string sfxParam   = "SFXVolume";

    private const float VolumeOn  =   0f;
    private const float VolumeOff = -80f;

    private const string MusicPrefKey = "MusicEnabled";
    private const string SFXPrefKey   = "SFXEnabled";

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        MusicEnabled = PlayerPrefs.GetInt(MusicPrefKey, 1) == 1;
        SFXEnabled   = PlayerPrefs.GetInt(SFXPrefKey,   1) == 1;

    }
    private void Start()
    {
        ApplyMusic(MusicEnabled);
        ApplySFX(SFXEnabled);
    }

    public void ToggleMusic()  => SetMusic(!MusicEnabled);
    public void ToggleSFX()    => SetSFX(!SFXEnabled);

    public void SetMusic(bool enabled)
    {
        MusicEnabled = enabled;
        PlayerPrefs.SetInt(MusicPrefKey, enabled ? 1 : 0);
        ApplyMusic(enabled);
        OnMusicToggled?.Invoke(enabled);
    }

    public void SetSFX(bool enabled)
    {
        SFXEnabled = enabled;
        PlayerPrefs.SetInt(SFXPrefKey, enabled ? 1 : 0);
        ApplySFX(enabled);
        OnSFXToggled?.Invoke(enabled);
    }

    private void ApplyMusic(bool enabled)
    {
        if (mixer != null)
            mixer.SetFloat(musicParam, enabled ? VolumeOn : VolumeOff);
    }

    private void ApplySFX(bool enabled)
    {
        if (mixer != null)
            mixer.SetFloat(sfxParam, enabled ? VolumeOn : VolumeOff);
    }
}
