using UnityEngine;
using UnityEngine.UI;
public class MeterBar : MonoBehaviour
{
   public Image dashFillImage; 
    
    public PlayerController player; 

    void Update()
    {
        // Calculate the percentage 
        float ratio = player.currentDashEnergy / player.maxDashEnergy;
        
        // Updates the bar
        dashFillImage.fillAmount = ratio;

        // Change color when full
        if (ratio >= 1.0f) {
            dashFillImage.color = Color.yellow; // Ready to dash
        } else {
            dashFillImage.color = Color.cyan;   // Charging
        }
    }

}
