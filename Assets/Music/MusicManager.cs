using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MusicManager : MonoBehaviour {
    [Header("BGM")]
    public AudioSource audioSource;
    public AudioClip backgroundMusic;
    public float bpm = 120f;
    private float beatLength;
    public float onBeatAccuracy = 0.1f;
    
    [Header("Delay setting")]
    [Tooltip("This is for testing delay. Positive means you're late; negative means you're early. Unit is %beat")]
    public double inputDelay;
    [Tooltip("Increase the value if delay > 0; decrease the value if delay < 0")] public float delayOffset = 0f; // 0.25 to 0.35 is generally good

    [Header("Components")]
    public GameObject player;

    private void Awake() {
        beatLength = 60f / bpm;
    }

    private void Start() {
        audioSource = GetComponent<AudioSource>();
        Invoke("PlayBgm", beatLength * 4 - delayOffset);
    }

    private void Update() {
        TestInputDelay();
    }

    public bool IsOnBeat() {
        var beatCount = Time.time / beatLength;
        var delay = beatCount - Math.Round(beatCount);
        return Math.Abs(delay) < onBeatAccuracy;
    }

    public float GetBeatLength() {
        return beatLength;
    }

    public float GetDelayOffset() {
        return delayOffset;
    }

    private void PlayBgm() {
        if (audioSource == null) return;
        audioSource.PlayOneShot(backgroundMusic);
    }

    private void TestInputDelay() {
        if (Input.GetKeyDown(KeyCode.J)) {
            var beatCount = Time.time / beatLength;
            inputDelay = beatCount - Math.Round(beatCount);
        }
    }
}
