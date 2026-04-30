using UnityEngine;

public class ObstacleSwingDriverFunnyTry : MonoBehaviour
{
    private HingeJoint _hinge;
    
    [Header("Swing Settings")]
    public float angleLimit = 45f;    
    public float motorSpeed = 200f;   
    public float motorForce = 1000f;

    private int _direction = 1;

    void Start()
    {
        _hinge = GetComponent<HingeJoint>();
        _hinge.useMotor = true;
        
        // Initial motor setup
        UpdateMotor(motorSpeed);
    }

    void FixedUpdate()
    {
        float currentAngle = _hinge.angle;


        if (_direction == 1 && currentAngle >= angleLimit)
        {
            _direction = -1;
            UpdateMotor(motorSpeed * _direction);
        }
        else if (_direction == -1 && currentAngle <= -angleLimit)
        {
            _direction = 1;
            UpdateMotor(motorSpeed * _direction);
        }
    }

    private void UpdateMotor(float speed)
    {
        JointMotor motor = _hinge.motor;
        motor.targetVelocity = speed;
        motor.force = motorForce;

        motor.freeSpin = false; 
        _hinge.motor = motor;
    }
}
