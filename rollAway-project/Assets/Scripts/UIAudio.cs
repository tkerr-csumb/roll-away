using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(AudioSource))]
public class UIAudio : MonoBehaviour
{
    public static UIAudio Instance { get; private set; }

    [SerializeField] private AudioClip clickClip;
    [SerializeField] private AudioClip hoverClip;
    [SerializeField] [Range(0f, 1f)] private float volume = 1f;

    private AudioSource _source;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _source = GetComponent<AudioSource>();
        _source.playOnAwake  = false;
        _source.loop         = false;
        _source.spatialBlend = 0f;
    }

    public void PlayClick()
    {
        if (clickClip != null)
            _source.PlayOneShot(clickClip, volume);
    }

    public void PlayHover()
    {
        if (hoverClip != null)
            _source.PlayOneShot(hoverClip, volume);
    }
    private void OnLevelWasLoaded(int level)
    {
        foreach (Button btn in FindObjectsByType<Button>(FindObjectsSortMode.None))
        {
            btn.onClick.AddListener(PlayClick);
        }
    }
}
