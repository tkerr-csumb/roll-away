using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private float movementX;
    private float movementY;
    public float speed;
    public TextMeshProUGUI countText;
    private int count;
    public GameObject winTextObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
     rb = GetComponent<Rigidbody>();   
     count = 0;
     SetCountText();
     winTextObject.SetActive(false);
    }
    void OnMove(InputValue movementValue){
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }
    void SetCountText() 
   {
       countText.text =  "Polyhedrons: " + count.ToString();
       if (count >= 12)
       {
           winTextObject.SetActive(true);
           Destroy(GameObject.FindGameObjectWithTag("Enemy"));
       }
   }
    void FixedUpdate(){
        Vector3 movement = new Vector3 (movementX, 0.0f, movementY);
        rb.AddForce(movement * speed);
    }
    void OnTriggerEnter(Collider other){
        if (other.gameObject.CompareTag("PickUp")){
        other.gameObject.SetActive(false);
        count = count +1;
        SetCountText();
       }
   }
   private void OnCollisionEnter(Collision collision)
{
   if (collision.gameObject.CompareTag("Enemy"))
   {
       // Destroy the current object
       Destroy(gameObject); 
       winTextObject.gameObject.SetActive(true);
       winTextObject.GetComponent<TextMeshProUGUI>().text = "HAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHA";
   }
}
}
