using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5; 
    public float smoothMoveTime = .1f;
    public float rotationSpeed = 10;
    float angle;
    float smoothInputMagnitude;
    float smoothMoveVelocity;
    Vector3 velocity;
    Rigidbody playerRb;
    Vector3 inputDirection;
    GameStates gameStates;
    public Vector3 cameraOffset;
    public static event System.Action OnGoalReached;

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        gameStates = GameObject.FindObjectOfType<GameStates>();
        gameStates.OnGameOver += FreezePlayer;
    }

    void Update()
    {
        GetInput();
        MoveCamera();     
    }

    void FixedUpdate()
    {
        MovePlayer();   
    }

    void GetInput()
    {
        inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        
        // inputMagnitude switches between 0 and 1
        float inputMagnitude = inputDirection.magnitude; 

        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMoveTime);
        velocity = transform.forward * speed * smoothInputMagnitude;
        
        float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        
        // Multiplied by inputMagnitude to stop from rotating to default orientation when no input present
        angle = Mathf.LerpAngle(angle, targetAngle, rotationSpeed * Time.deltaTime * inputMagnitude); 
    }

    void MovePlayer()
    {
        playerRb.MoveRotation(Quaternion.Euler(Vector3.up * angle));
        playerRb.MovePosition(playerRb.position + velocity * Time.deltaTime);
    }
    void MoveCamera()
    {
        Camera.main.transform.position = transform.position + cameraOffset;
    }

    void FreezePlayer()
    {
        speed = 0;
        rotationSpeed = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Goal")
        {
            if (OnGoalReached != null)
            {
                OnGoalReached();
            };
        }
    }

    private void OnDestroy()
    {
        gameStates.OnGameOver -= FreezePlayer;
    }
}
