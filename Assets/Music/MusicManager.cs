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
    public float globalDelay = 0f;
    
    [Header("Components")]
    public GameObject player;
 
    [Header("Others")]
    private bool isPlaying = false;

    private void Awake() {
        beatLength = 60f / bpm;
    }

    private void Start() {
        audioSource = GetComponent<AudioSource>();
        isPlaying = false;
    }

    private void Update() {
        TestInputDelay();
        if (!isPlaying && Input.GetKeyDown(KeyCode.P)) {
            Invoke("PlayBgm", beatLength * 4 - delayOffset);
            isPlaying = true;
            globalDelay = Time.time;
        }
    }

    public float BeatCount() {
        return (Time.time - globalDelay) / beatLength;
    }
    
    public double DifferenceFromBeat() {
        var beatCount = BeatCount();
        return beatCount - Math.Round(beatCount);
        // var dif = Time.time - globalDelay - Math.Round(beatCount) * beatLength;
        // return dif;
    }

    public bool IsOnBeat() {
        return Math.Abs(DifferenceFromBeat()) < onBeatAccuracy;
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
        Debug.Log("BGM playing");
    }

    private void TestInputDelay() {
        if (Input.GetKeyDown(KeyCode.J)) {
            inputDelay = DifferenceFromBeat();
        }
    }
}
