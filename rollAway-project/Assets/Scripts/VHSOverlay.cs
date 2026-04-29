using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class VHSOverlay : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera gameCamera;

    [Header("UI Elements (inside Pause panel)")]
    [SerializeField] private RawImage frozenFrameImage;
    [SerializeField] private RawImage vhsOverlayImage;

    [Header("VHS Video Clip")]
    [SerializeField] private VideoClip vhsClip;

    [Header("Chroma Key Material")]
    [SerializeField] private Material chromaKeyMaterial;

    [Header("Render Texture Resolution")]
    [SerializeField] private int rtWidth  = 1920;
    [SerializeField] private int rtHeight = 1080;

    private VideoPlayer _videoPlayer;
    private RenderTexture _frozenRT;
    private RenderTexture _videoRT;

    private void Awake()
    {
        _videoPlayer = GetComponent<VideoPlayer>();

        _frozenRT = new RenderTexture(rtWidth, rtHeight, 0, RenderTextureFormat.ARGB32);
        _videoRT  = new RenderTexture(rtWidth, rtHeight, 0, RenderTextureFormat.ARGB32);

        _videoPlayer.renderMode    = VideoRenderMode.RenderTexture;
        _videoPlayer.targetTexture = _videoRT;
        _videoPlayer.isLooping     = true;
        _videoPlayer.playOnAwake   = false;

        if (vhsClip != null)
            _videoPlayer.clip = vhsClip;

        if (frozenFrameImage != null)
            frozenFrameImage.texture = _frozenRT;

        if (vhsOverlayImage != null)
        {
            vhsOverlayImage.texture  = _videoRT;
            vhsOverlayImage.material = chromaKeyMaterial;
        }

        SetVisible(false);
    }

    private void OnEnable()
    {
        PauseManager.OnPaused  += HandlePause;
        PauseManager.OnResumed += HandleResume;
    }

    private void OnDisable()
    {
        PauseManager.OnPaused  -= HandlePause;
        PauseManager.OnResumed -= HandleResume;
    }

    private void OnDestroy()
    {
        if (_frozenRT != null) _frozenRT.Release();
        if (_videoRT  != null) _videoRT.Release();
    }

    private void HandlePause()
    {
        CaptureFrame();
        _videoPlayer.Play();
        SetVisible(true);
    }

    private void HandleResume()
    {
        _videoPlayer.Stop();
        SetVisible(false);
    }

    private void CaptureFrame()
    {
        if (gameCamera == null || _frozenRT == null) return;

        RenderTexture previous    = gameCamera.targetTexture;
        gameCamera.targetTexture  = _frozenRT;
        gameCamera.Render();
        gameCamera.targetTexture  = previous;
    }

    private void SetVisible(bool visible)
    {
        if (frozenFrameImage != null) frozenFrameImage.gameObject.SetActive(visible);
        if (vhsOverlayImage  != null) vhsOverlayImage.gameObject.SetActive(visible);
    }
}
