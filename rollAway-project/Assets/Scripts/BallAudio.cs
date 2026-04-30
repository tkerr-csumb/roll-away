using UnityEngine;

public class BallAudio : MonoBehaviour
{
    [Header("Rolling Clips (per surface)")]
    [SerializeField]
    private AudioClip rollNormal;

    [SerializeField]
    private AudioClip rollIce;

    [SerializeField]
    private AudioClip rollRubber;

    [SerializeField]
    private AudioClip rollSticky;

    [Header("One-Shot Clips")]
    [SerializeField]
    private AudioClip landingClip;

    [SerializeField]
    private AudioClip jumpClip;

    [SerializeField]
    private AudioClip dashClip;

    [SerializeField]
    private AudioClip gravityFlipClip;

    [Header("Rolling Tuning")]
    [SerializeField]
    private float rollSpeedMin = 0.5f;

    [SerializeField]
    private float rollSpeedMax = 12f;

    [SerializeField]
    private float rollVolumeMax = 0.8f;

    [SerializeField]
    private float rollPitchMin = 0.8f;

    [SerializeField]
    private float rollPitchMax = 1.3f;

    [Header("Landing Tuning")]
    [SerializeField]
    private float landingImpactThreshold = 5f;

    private AudioSource _rollSource;
    private AudioSource _sfxSource;
    private Rigidbody _rb;
    private PlayerController _player;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _player = GetComponent<PlayerController>();

        AudioSource[] sources = GetComponents<AudioSource>();
        if (sources.Length < 2)
        {
            Debug.LogError(
                "BallAudio needs 2 AudioSource components on this GameObject. "
                    + "Add them via Add Component in the Inspector."
            );
            return;
        }
        _rollSource = sources[0];
        _sfxSource = sources[1];

        _rollSource.loop = true;
        _rollSource.playOnAwake = false;
        _rollSource.spatialBlend = 1f;
        _sfxSource.loop = false;
        _sfxSource.playOnAwake = false;
        _sfxSource.spatialBlend = 1f;
    }

    private void OnEnable()
    {
        GravityControl.OnGravityFlipped += PlayGravityFlip;
    }

    private void OnDisable()
    {
        GravityControl.OnGravityFlipped -= PlayGravityFlip;
    }

    private void Update()
    {
        if (_rollSource == null)
            return;

        float speed = _rb.linearVelocity.magnitude;

        AudioClip targetClip = GetRollClip(_player.CurrentSurface);
        if (_rollSource.clip != targetClip)
        {
            _rollSource.clip = targetClip;
            if (
                speed > rollSpeedMin
                && (
                    _player.IsGrounded
                    || _player.CurrentSurface == PlayerController.SurfaceType.Sticky
                )
            )
                _rollSource.Play();
        }

        if (
            speed > rollSpeedMin
            && (_player.IsGrounded || _player.CurrentSurface == PlayerController.SurfaceType.Sticky)
        )
        {
            if (!_rollSource.isPlaying)
                _rollSource.Play();

            float t = Mathf.InverseLerp(rollSpeedMin, rollSpeedMax, speed);
            _rollSource.volume = Mathf.Lerp(0f, rollVolumeMax, t);
            _rollSource.pitch = Mathf.Lerp(rollPitchMin, rollPitchMax, t);
        }
        else
        {
            if (
                !_player.IsGrounded
                && _player.CurrentSurface != PlayerController.SurfaceType.Sticky
            )
            {
                _rollSource.volume = 0f;
                if (_rollSource.isPlaying)
                    _rollSource.Pause();
            }
            else
            {
                _rollSource.volume = Mathf.MoveTowards(_rollSource.volume, 0f, Time.deltaTime * 4f);
                if (_rollSource.volume <= 0f && _rollSource.isPlaying)
                    _rollSource.Pause();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_sfxSource == null)
            return;
        if (collision.relativeVelocity.magnitude > landingImpactThreshold && landingClip != null)
            _sfxSource.PlayOneShot(landingClip);
    }

    public void PlayJump()
    {
        if (_sfxSource != null && jumpClip != null)
            _sfxSource.PlayOneShot(jumpClip);
    }

    public void PlayDash()
    {
        if (_sfxSource != null && dashClip != null)
            _sfxSource.PlayOneShot(dashClip);
    }

    private void PlayGravityFlip()
    {
        if (_sfxSource != null && gravityFlipClip != null)
            _sfxSource.PlayOneShot(gravityFlipClip);
    }

    private AudioClip GetRollClip(PlayerController.SurfaceType surface)
    {
        return surface switch
        {
            PlayerController.SurfaceType.Ice => rollIce != null ? rollIce : rollNormal,
            PlayerController.SurfaceType.Rubber => rollRubber != null ? rollRubber : rollNormal,
            PlayerController.SurfaceType.Sticky => rollSticky != null ? rollSticky : rollNormal,
            _ => rollNormal,
        };
    }
}
