using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class CameraManager : MonoBehaviour {
    [Header("Components")]
    public GameObject currentVCam;
    public Camera mainCam;
    private CinemachineBrain brain;

    [Header("Virtual Cameras")]
    public CinemachineVirtualCamera[] vCamList;

    [Header("Settings")]
    public float normalRotX = 30f;
    public float focusRotX = 50f;
    public float transitionTime = 0.3f;

    [Header("Freelook Camera")]
    public float sensitivityX = 8f;
    [SerializeField][Range(0.01f, 0.1f)] public float sensitivityZoom = 0.3f;
    public bool debugCamera;
    private float mouseX;
    private float zoom;

    [Header("Current Info")]
    public bool isFocus;

    private void Awake() {
        brain = mainCam.GetComponent<CinemachineBrain>();
    }

    private void Start() {
        Cursor.visible = false;
        vCamList = FindObjectsOfType<CinemachineVirtualCamera>();
        if (vCamList != null) {
            foreach (var cam in vCamList) {
                cam.Priority = 10;
            }
        }
    }

    private void Update() {
        Cursor.visible = false;

        if (debugCamera) FreeLook();
        if (brain != null) currentVCam = brain.ActiveVirtualCamera.VirtualCameraGameObject;
    }

    private void SetCameraFocus(bool isFocus) {
        if (currentVCam == null) return;

        var targetAngle = currentVCam.transform.eulerAngles;
        targetAngle.x = isFocus ? focusRotX : normalRotX;

        currentVCam.transform.DORotate(targetAngle, transitionTime);
    }
    
    private void FreeLook() {
        if (currentVCam == null) return;
        
        var vCamEuler = currentVCam.transform.localEulerAngles;
        var currentX = vCamEuler.x;
        var currentY = vCamEuler.y;
        var currentZ = vCamEuler.z;
        
        // Rotate
        currentVCam.transform.localEulerAngles = new Vector3(currentX, currentY + mouseX * sensitivityX * Time.deltaTime, currentZ);

        // Zoom
        var vCam = currentVCam.GetComponent<CinemachineVirtualCamera>();
        vCam.m_Lens.OrthographicSize = Mathf.Clamp(vCam.m_Lens.OrthographicSize + zoom * sensitivityZoom, 2.1f, 15f);
    }

    public void SetActiveVCam(GameObject vCam) {
        var cineMachine = vCam.GetComponent<CinemachineVirtualCamera>();
        var setting = vCam.GetComponent<VirtualCameraSetting>();

        foreach (var cam in vCamList) {
            if (cam != cineMachine) cam.Priority = 10;
        }

        vCam.GetComponent<CinemachineVirtualCamera>().Priority = 20;
        mainCam.cullingMask = setting.layerMask;
    }
    
    public void OnFocus(InputAction.CallbackContext context) {
        if (context.performed) {
            SetCameraFocus(true);
            isFocus = true;
        }
        else if (context.canceled) {
            SetCameraFocus(false);
            isFocus = false;
        }
    }
    
    public void OnLookX(InputAction.CallbackContext context) {
        mouseX = context.ReadValue<float>();
    }

    public void OnZoom(InputAction.CallbackContext context) {
        zoom = -context.ReadValue<Vector2>().y;
    }
}
