using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Teleport : MonoBehaviour {
    [Header("Components")]
    public Transform destination;
    public GameObject player;
    public GameObject playerPrefab;
    public MeshRenderer platform;
    public Material normalMat;
    public Material shinyMat;

    [Header("Settings")]
    public float lingerTime = 2f;
    public float timer = 0f;

    private void Update() {
        if (timer >= lingerTime) TeleportPlayer();
    }

    private void TeleportPlayer() {

        player.GetComponent<NavMeshAgent>().Warp(destination.position);
        timer = 0f;

        // player.GetComponent<NavMeshAgent>().enabled = false;
        // player.transform.position = destination.position;
        // player.transform.rotation = destination.rotation;
        // player.GetComponent<NavMeshAgent>().enabled = true;
        // player.GetComponent<NavMeshAgent>().ResetPath();


        // Destroy(player);
        // Instantiate(playerPrefab, destination.position, destination.rotation);
    }

    private void OnTriggerStay(Collider other) {
        if (!other.gameObject.CompareTag("Player")) return;

        timer += Time.deltaTime;

        if (platform == null) return;

        platform.material = shinyMat;
    }

    private void OnTriggerExit(Collider other) {
        if (!other.gameObject.CompareTag("Player")) return;
        if (timer > lingerTime) return;
        
        timer = 0f;
        
        if (platform == null) return;

        platform.material = normalMat;
    }
}
