using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class VirtualCameraSetting : MonoBehaviour {
    [Header("Component")]
    private CinemachineVirtualCamera vCam;
    
    [Header("Camera Setting")]
    public float camRotY = 45f;
    public float orthoSize = 8f;
    public LayerMask layerMask;

    private void Awake() {
        vCam = GetComponent<CinemachineVirtualCamera>();
    }

    private void Start() {
        InitializeVCam();
    }

    private void InitializeVCam() {
        if (vCam == null) return;
        vCam.transform.eulerAngles = new Vector3(30f, camRotY, 0f);
        vCam.m_Lens.OrthographicSize = orthoSize;
    }
}
