using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraTransitionTrigger : MonoBehaviour {
    [Header("Components")]
    public CameraManager manager;
    public GameObject enterVCam;
    public GameObject exitVcam;

    private void OnTriggerEnter(Collider other) {
        Debug.Log("Hey");
        if (!other.gameObject.CompareTag("Player")) return;
        if (enterVCam == null) return;
        
        manager.SetActiveVCam(enterVCam);
    }

    private void OnTriggerExit(Collider other) {
        Debug.Log("hi");

        if (!other.gameObject.CompareTag("Player")) return;
        if (exitVcam == null) return;
        
        manager.SetActiveVCam(exitVcam);
    }

    
}
