using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targetable : MonoBehaviour {
    [SerializeField] public GameObject lockOnSign;
    [SerializeField] private bool isTargeted = false;

    private void Start() {
        if (lockOnSign != null) lockOnSign.SetActive(false);
    }

    public bool IsTargetable() {
        return true;
    }

    public void OnTargeted() {
        isTargeted = true;
        if (lockOnSign != null) lockOnSign.SetActive(true);
    }

    public void OnTargetLost() {
        isTargeted = false;
        if (lockOnSign != null) lockOnSign.SetActive(false);
    }
    
    public bool IsTargeted() {
        return isTargeted;
    }
}
