using UnityEngine;

[DisallowMultipleComponent]
public class PauseResponder : MonoBehaviour
{
    [Header("Optional auto-handled components")]
    [SerializeField] private bool freezeRigidbody      = true;
    [SerializeField] private bool pauseAnimator         = true;
    [SerializeField] private bool disableMonoBehaviours = false;

    [SerializeField] private MonoBehaviour[] behavioursToToggle;

    private Rigidbody   _rb;
    private Rigidbody2D _rb2d;
    private Animator    _anim;
    private Vector3     _storedVelocity;
    private Vector3     _storedAngularVelocity;

    private void Awake()
    {
        _rb   = GetComponent<Rigidbody>();
        _rb2d = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
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

    private void HandlePause()
    {
        if (freezeRigidbody)
        {
            if (_rb != null)
            {
                _storedVelocity        = _rb.linearVelocity;
                _storedAngularVelocity = _rb.angularVelocity;
                _rb.linearVelocity     = Vector3.zero;
                _rb.angularVelocity    = Vector3.zero;
                _rb.isKinematic        = true;
            }
            if (_rb2d != null)
            {
                _rb2d.linearVelocity  = Vector2.zero;
                _rb2d.angularVelocity = 0f;
                _rb2d.isKinematic     = true;
            }
        }

        if (pauseAnimator && _anim != null)
            _anim.speed = 0f;

        if (disableMonoBehaviours)
            foreach (var mb in behavioursToToggle)
                if (mb) mb.enabled = false;
    }

    private void HandleResume()
    {
        if (freezeRigidbody)
        {
            if (_rb != null)
            {
                _rb.isKinematic     = false;
                _rb.linearVelocity  = _storedVelocity;
                _rb.angularVelocity = _storedAngularVelocity;
            }
            if (_rb2d != null)
            {
                _rb2d.isKinematic = false;
            }
        }

        if (pauseAnimator && _anim != null)
            _anim.speed = 1f;

        if (disableMonoBehaviours)
            foreach (var mb in behavioursToToggle)
                if (mb) mb.enabled = true;
    }
}
