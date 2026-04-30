using System;
using UnityEngine;

public class PlayerDashEnergy : MonoBehaviour
{
    [Header("Dash Energy")]
    [NonSerialized]
    public float maxDashEnergy = 100f;

    [NonSerialized]
    public float currentDashEnergy = 0f;

    [NonSerialized]
    public float energyGainMultiplier = 2f;

    [Header("Grounding")]
    [SerializeField]
    private PlayerController playerController;

    private Vector3 lastPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lastPosition = transform.position;
        if (playerController == null)
            playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;

        //gain dash energy from movement
        currentDashEnergy += distanceMoved * energyGainMultiplier;
        currentDashEnergy = Mathf.Clamp(currentDashEnergy, 0f, maxDashEnergy);

        // Debug.Log($"Grounded: {playerController?.IsGrounded}, Energy: {currentDashEnergy}");
    }

    public bool HasEnergy()
    {
        return currentDashEnergy >= maxDashEnergy;
    }

    public void ConsumeAlleEnergy()
    {
        currentDashEnergy = 0f;
    }

    public float GetCurrentDashEnergy()
    {
        return currentDashEnergy;
    }

    public float GetNormalizedEnergy()
    {
        if (maxDashEnergy <= 0f)
            return 0f;
        return currentDashEnergy / maxDashEnergy;
    }
}
