using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]

public class PlayerController : MonoBehaviour {
    [Header("Camera")]
    public CinemachineBrain cinemachineBrain;
    public Transform currentVcam;
    public float camAngleNormal = 30f;
    public float camAngleFocus = 50f;
    public float focusTransitionTime = 0.3f;
    public float rotateTransitionTime = 0.5f;
    public bool isFocus;
    private bool cameraRotateState; // 0 for facing northeast, 1 for facing northwest

    // public float sensitivityX = 8f;
    // public bool debugCamera;
    // private float mouseX;

    [Header("Components")]
    private NavMeshAgent agent;

    [Header("Movement Setting")]
    public float walkSpeed = 6f;
    public float runSpeed = 8f;
    public float currentSpeed;

    [Header("Current Movement Info")]
    public Vector2 input;
    public Vector3 velocity;
    public bool isRunning;
    private Vector3 moveDirection;
    
    [Header("Others")]
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start() {
        currentSpeed = walkSpeed;
        cameraRotateState = false;
    }

    private void Update() {
        MoveByNavMesh();
        currentVcam = cinemachineBrain.ActiveVirtualCamera.VirtualCameraGameObject.transform;
    }

    private void MoveByNavMesh() {
        currentSpeed = isRunning ? runSpeed : walkSpeed;
        
        if (input.magnitude >= 0.1f) {
            // Rotate 
            var cameraAngleOffset = currentVcam.eulerAngles;
            var targetAngle = cameraAngleOffset.y + Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg;
            var currentAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity,
                turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, currentAngle, 0f);
        
            // Move 
            moveDirection = Quaternion.Euler(0, cameraAngleOffset.y, 0) * new Vector3(input.x, 0f, input.y);
            velocity.x = moveDirection.x * currentSpeed;
            velocity.z = moveDirection.z * currentSpeed;
        }
        else {
            velocity.x = 0;
            velocity.z = 0;
        }
        
        agent.Move(velocity * Time.deltaTime);
    }

    // private void SetCameraFocus(bool focusState) {
    //     var targetAngle = currentVcam.eulerAngles;
    //     targetAngle.x = focusState ? camAngleFocus : camAngleNormal;
    //     currentVcam.DORotate(targetAngle, focusTransitionTime);
    // }

    private void SetCameraRotate() {
        var targetAngle = currentVcam.eulerAngles;
        targetAngle.y = cameraRotateState ? -45f : 45f;
        currentVcam.DORotate(targetAngle, rotateTransitionTime);
        cameraRotateState = !cameraRotateState;
    }

    // private void FreeLook() {
    //     var currentX = currentVcam.localEulerAngles.x;
    //     var currentY = currentVcam.localEulerAngles.y;
    //     var currentZ = currentVcam.localEulerAngles.z;
    //     currentVcam.localEulerAngles = new Vector3(currentX, currentY + mouseX * sensitivityX * Time.deltaTime, currentZ);
    // }
    

    public void OnMove(InputAction.CallbackContext context) {
        input = context.ReadValue<Vector2>();
        moveDirection = new Vector3(input.x, 0f, input.y).normalized;
    }

    // public void OnFocus(InputAction.CallbackContext context) {
    //     if (context.performed) {
    //         SetCameraFocus(true);
    //         isFocus = true;
    //     }
    //     else if (context.canceled) {
    //         SetCameraFocus(false);
    //         isFocus = false;
    //     }
    // }

    public void OnCameraRotate(InputAction.CallbackContext context) {
        if (context.started) SetCameraRotate();
    }

    public void OnRun(InputAction.CallbackContext context) {    
        // When run button is down, smoothly accelerate to run speed TODO
    }

    public void OnExitGame(InputAction.CallbackContext context) {
        if (context.started) Application.Quit();
    }

    // public void OnLookX(InputAction.CallbackContext context) {
    //     mouseX = context.ReadValue<float>();
    // }

    public void OnSprint(InputAction.CallbackContext context) {
        if (context.performed) isRunning = true;
        else if (context.canceled) isRunning = false;
    }
}
