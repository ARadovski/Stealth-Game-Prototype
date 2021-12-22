using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5; 
    public float smoothMoveTime = .1f;
    public float rotationSpeed = 10;
    float targetAngle;
    float angle;
    float smoothInputMagnitude;
    float smoothMoveVelocity;
    Vector3 velocity;
    Rigidbody playerRb;
    Vector3 inputDirection;
    public Vector3 cameraOffset;

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        float inputMagnitude = inputDirection.magnitude;
        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMoveTime);
        velocity = transform.forward * speed * smoothInputMagnitude;
        
        targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        angle = Mathf.LerpAngle(angle, targetAngle, rotationSpeed * Time.deltaTime * inputMagnitude);

        Camera.main.transform.position = transform.position + cameraOffset;
       
    }

    void FixedUpdate()
    {
        // Multiplying by magnitude to prevent direction from snapping to default orientation on key release
        playerRb.MoveRotation(Quaternion.Euler(Vector3.up * angle));
        playerRb.MovePosition(playerRb.position + velocity * Time.deltaTime);
    }
}
