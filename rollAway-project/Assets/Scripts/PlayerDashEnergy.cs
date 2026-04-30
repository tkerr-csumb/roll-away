using UnityEngine;

public class PlayerDashEnergy : MonoBehaviour
{
    [Header("Dash Energy")] 
    [SerializeField] private float maxDashEnergy = 100f;
    [SerializeField] private float currentDashEnergy = 0f;
    [SerializeField] private float energyGainMultiplier = 2f;

    [Header("Grounding")] [SerializeField] private PlayerController playerController;

    private Vector3 lastPosition;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lastPosition = transform.position;
        if(playerController == null)
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
    }
    
    // void Update()
    // {
    //     // Calculate the distance covered (ignoring vertical movement)
    //     Vector3 currentPosFlat = new Vector3(transform.position.x, 0, transform.position.z);
    //     Vector3 lastPosFlat = new Vector3(lastPosition.x, 0, lastPosition.z);
    //     float distanceMoved = Vector3.Distance(currentPosFlat, lastPosFlat);
    //     // Only add energy if touching the ground
    //     if (isGrounded && currentDashEnergy < maxDashEnergy)
    //     {
    //         currentDashEnergy += distanceMoved * energyGainMultiplier;
    //
    //         // Keep it from going over the max
    //         currentDashEnergy = Mathf.Clamp(currentDashEnergy, 0, maxDashEnergy);
    //     }
    //
    //     lastPosition = transform.position;
    // }

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
        if(maxDashEnergy<=0f)
            return 0f;
        return currentDashEnergy / maxDashEnergy;
    }
}
