using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metronome : MonoBehaviour {
    public MusicManager musicManager;
    public GameObject indicator;
    public GameObject countDownText;
    public float twinkleDuration = 0.2f;
    public float countDownDuration = 1f;
    public bool canTwinkle = false;
    public bool isPlaying = false;
    public float twinkleOffset;

    private void Start() {
        if (indicator) indicator.SetActive(false);
        if (countDownText) countDownText.SetActive(false);
    }

    private void Update()
    {
        if (!isPlaying && Input.GetKeyDown(KeyCode.P)) {
            StartCoroutine(ShowCountDown());
            Invoke("EnableTwinkle", musicManager.GetBeatLength() * 4);
            isPlaying = true;
        }
        
        if (!canTwinkle) return;
        
        if (musicManager.IsOnBeat() && !indicator.activeSelf) {
            StartCoroutine(Twinkle());
        }
    }

    private IEnumerator Twinkle() {
        yield return new WaitForSeconds(twinkleOffset);
        indicator.SetActive(true);
        Debug.Log("Time: " + Time.time + "; " + "BeatCount: " + musicManager.BeatCount());
        yield return new WaitForSeconds(twinkleDuration);
        indicator.SetActive(false);
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
