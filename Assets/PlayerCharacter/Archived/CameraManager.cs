using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour {
    [Header("Components")]
    public Transform currentVcam;

    [Header("Settings")]
    public float camAngleNormal = 30f;
    public float camAngleFocus = 50f;
    public float transitionTime = 0.3f;
    
    private void SetCameraFocus(bool isFocus) {
        var targetAngle = currentVcam.eulerAngles;
        targetAngle.x = isFocus ? camAngleFocus : camAngleNormal;

        currentVcam.DORotate(targetAngle, transitionTime);
    }
    
    public void OnFocus(InputAction.CallbackContext context) {
        // isFocus = context.ReadValue<float>() > 0f ? true : false;
        if (context.performed) {
            SetCameraFocus(true);
        }
        else if (context.canceled) {
            SetCameraFocus(false);
        }
    }
}
