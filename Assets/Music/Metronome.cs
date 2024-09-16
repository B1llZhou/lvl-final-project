using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metronome : MonoBehaviour {
    public MusicManager musicManager;
    public GameObject[] indicators;
    public GameObject countDownText;
    public GameObject instructionText;
    public float twinkleDuration = 0.2f;
    public float countDownDuration = 1f;
    public bool canTwinkle = false;
    public bool isPlaying = false;
    public float twinkleOffset;

    [Header("Trigger")] 
    public bool isTriggerActive = false;

    private void Start() {
        if (indicators != null)
        {
            foreach (var i in indicators)
            {
                i.SetActive(false);
            }
        }
        if (countDownText) countDownText.SetActive(false);
        if (instructionText) instructionText.SetActive(true);
    }

    private void Update()
    {
        if (!isPlaying)
        {
            if (Input.GetKeyDown(KeyCode.P) || isTriggerActive) {
                if (instructionText) instructionText.SetActive(false);
                StartCoroutine(ShowCountDown());
                Invoke("EnableTwinkle", musicManager.GetBeatLength() * 4);
                isPlaying = true;
            }
        }
        
        
        if (!canTwinkle) return;
        
        if (musicManager.IsOnBeat() /*&& !indicator.activeSelf*/) {
            StartCoroutine(Twinkle());
        }
    }

    private IEnumerator Twinkle() {
        yield return new WaitForSeconds(twinkleOffset);
        foreach (var i in indicators)
        {
            i.SetActive(true);
        }
        // Debug.Log("Time: " + Time.time + "; " + "BeatCount: " + musicManager.BeatCount());
        yield return new WaitForSeconds(twinkleDuration);
        foreach (var i in indicators)
        {
            i.SetActive(false);
        }    
    }

    private void EnableTwinkle() {
        canTwinkle = true;
        Debug.Log("Twinkle enabled");
    }

    private IEnumerator ShowCountDown() {
        countDownText.SetActive(true);
        yield return new WaitForSeconds(countDownDuration);
        countDownText.SetActive(false);
    }
}
