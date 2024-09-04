using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Ability : MonoBehaviour
{
    public GameObject model;
    public float attackDuration;
    public List<AudioClip> audioClips = new List<AudioClip>();
    public AudioSource audioSource;
    private Coroutine releaseCo;

    [Header("Music")]
    private float beatLength;
    public MusicManager musicManager;
    private float delayOffset = 0f;

    private void Start()
    {
        model.gameObject.SetActive(false);

        musicManager = FindObjectOfType<MusicManager>();
        beatLength = musicManager.GetBeatLength();
        delayOffset = musicManager.GetDelayOffset();
    }

    public bool Release(int comboSeq = 0) {
        if (releaseCo != null) {
            //SetAbilityOn(false);
            //releaseCo = null;   
            return false;
        }
        
        releaseCo = StartCoroutine(ReleaseCo(comboSeq));

        return true;
    }

    private IEnumerator ReleaseCo(int comboSeq = 0) {
        attackDuration = comboSeq switch {
            0 => beatLength * 2,
            1 => beatLength * 2,
            2 => beatLength * 4,
            _ => beatLength * 2
        };

        SetAbilityOn(true, comboSeq);
        // Debug.Log("Start atk");
        // musicManager.IsOnBeat();
        // Debug.Log("Atk START: " + musicManager.delay);
        
        yield return new WaitForSeconds(attackDuration);
        
        SetAbilityOn(false);
        // musicManager.IsOnBeat();
        // Debug.Log("Atk END: " + musicManager.delay);
        
        releaseCo = null;
    }

    private void SetAbilityOn(bool b, int comboSeq = 0) {
        if (model != null) {
            model.gameObject.SetActive(b);
        }

        if (b && audioSource != null) {
            if (comboSeq < audioClips.Count) {
                audioSource.PlayOneShot(audioClips[comboSeq]);
            } else {
                audioSource.PlayOneShot(audioClips[audioClips.Count-1]);
            }
        }
    }
}