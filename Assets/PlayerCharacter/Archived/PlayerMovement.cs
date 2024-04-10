using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    [Header("Components")]
    public CharacterController controller;
    public Transform followCamera;

    [Header("Movement Stats")]
    public float maxSpeed = 6f;
    public float gravityScale = 1f;

    [Header("Current Info")]
    public Vector2 inputDirection;
    public Vector3 velocity;
    public bool isGrounded;
    private Vector3 moveDirection;

    [Header("Others")]
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    private void Update() {
        isGrounded = controller.isGrounded;
        Movement();
    }

    private void Movement() {
        // Gravity
        if (!isGrounded) {
            velocity.y += Physics.gravity.y * gravityScale * Time.fixedDeltaTime;
        }
        else {
            velocity.y = 0f;
        }

        if (inputDirection.magnitude >= 0.1f) {
            // Rotate 
            var cameraAngleOffset = followCamera.eulerAngles;
            var targetAngle = cameraAngleOffset.y + Mathf.Atan2(inputDirection.x, inputDirection.y) * Mathf.Rad2Deg;
            var currentAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity,
                turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, currentAngle, 0f);
        
            // Move 
            moveDirection = Quaternion.Euler(0, cameraAngleOffset.y, 0) * new Vector3(inputDirection.x, 0f, inputDirection.y);
            velocity.x = moveDirection.x * maxSpeed;
            velocity.z = moveDirection.z * maxSpeed;
        }
        else {
            velocity.x = 0;
            velocity.z = 0;
        }
        
        controller.Move(velocity * Time.deltaTime);
    }
    
    public void OnMovement(InputAction.CallbackContext context) {
        inputDirection = context.ReadValue<Vector2>();
    }
}
