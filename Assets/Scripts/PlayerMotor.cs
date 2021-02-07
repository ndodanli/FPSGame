using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    private Vector3 velocity = Vector3.zero;
    private Vector3 rotationPlayer = Vector3.zero;
    private float rotationCameraX = 0f;
    private Rigidbody rb;
    private Vector3 thrusterForce = Vector3.zero;
    private float currentCameraRotationX = 0f;

    [SerializeField]
    private float cameraRotationLimit = 85f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 velocity)
    {
        this.velocity = velocity;
    }

    public void Rotate(Vector3 rotationPlayer, float rotationCameraX)
    {
        this.rotationPlayer = rotationPlayer;
        this.rotationCameraX = rotationCameraX;
    }


    private void FixedUpdate()
    {
        PerformMovement();
    }
    //run every physics iteration
    void LateUpdate()
    {
        PerformRotation();
        
        
    }

    //perform movement based on velocity variable
    void PerformMovement()
    {
        if (velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }

        if (thrusterForce != Vector3.zero)
        {
            rb.AddForce(thrusterForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }

    void PerformRotation()
    {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotationPlayer));
        if (cam != null)
        {
            //set our rotation and clamp it
            currentCameraRotationX -= rotationCameraX;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);
            cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
        }
    }

    //get a force vector for our thrusters
    public void ApplyThruster(Vector3 thrusterForce)
    {
        this.thrusterForce = thrusterForce;
    }
}
