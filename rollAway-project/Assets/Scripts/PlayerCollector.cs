using System;
using UnityEngine;
using TMPro;
public class PlayerCollector : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private GameObject winTextObject;

    [Header("Win Settings")] 
    [SerializeField] private int targetPickupCount = 12;

    [SerializeField] private string pickupTag = "PickUp";
    [SerializeField] private string enemyTag = "Enemy";
    
    private int count = 0;
    
        
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateCountUI();
        if(winTextObject != null)
                winTextObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag(pickupTag))
            return;
        other.gameObject.SetActive(false);
        count++;
        UpdateCountUI();
        
        
    }

    private void UpdateCountUI()
    {
        if (countText != null)
            countText.text = "Polyhedrons: " + count.ToString();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
