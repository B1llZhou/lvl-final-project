using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Ability : MonoBehaviour
{
    [Header("Components")]
    public GameObject[] hitboxes;
    public float attackDuration;
    public List<AudioClip> audioClips = new List<AudioClip>();
    public AudioSource audioSource;
    private Coroutine releaseCo;

    [Header("Music")]
    private float beatLength;
    public MusicManager musicManager;
    private float delayOffset = 0f;

    [Header("Attack Property")] 
    [SerializeField]
    private bool attackFirst = false;

    private void Start()
    {
        foreach (var v in hitboxes)
        {
            v.SetActive(false);
        }

        musicManager = FindObjectOfType<MusicManager>();
        beatLength = musicManager.GetBeatLength();
        delayOffset = musicManager.GetDelayOffset();
    }

    public bool Release(int comboSeq = 0, float skipTime = 0f) {
        // if (releaseCo != null) {
        //     //SetAbilityOn(false);
        //     //releaseCo = null;   
        //     return false;
        // }
        
        // releaseCo = StartCoroutine(ReleaseCo(comboSeq));
        
        StartCoroutine(ReleaseCo(comboSeq, skipTime));
        return true;
    }

    private IEnumerator ReleaseCo(int comboSeq = 0, float skipTime = 0f) {
        attackDuration = comboSeq switch {
            0 => beatLength * 2,
            1 => beatLength * 2,
            2 => beatLength * 4,
            _ => beatLength * 2
        };

        SetAbilityOn(true, comboSeq, skipTime);
        yield return new WaitForSeconds(attackDuration - skipTime);
        SetAbilityOn(false, comboSeq, skipTime);
        
        releaseCo = null;
    }

    private void SetAbilityOn(bool b, int comboSeq = 0, float skipTime = 0f) {
        if (hitboxes != null) {
            hitboxes[comboSeq].SetActive(b);
            StartCoroutine(HiboxChange(comboSeq));
        }

        if (b && audioSource != null) {
            if (comboSeq < audioClips.Count)
            {
                audioSource.clip = audioClips[comboSeq];
                if (skipTime != 0) audioSource.time = skipTime;
                audioSource.Play();
                // Debug.Log(skipTime);
            } 
            else {
                audioSource.clip = audioClips[^1];
                if (skipTime != 0) audioSource.time = skipTime;
                audioSource.Play();
            }
        }
    }

    private IEnumerator HiboxChange(int comboSeq)
    {
        var waitTime = comboSeq switch
        {
            0 => beatLength * 1,
            1 => beatLength * 1,
            2 => beatLength * 2,
            _ => beatLength * 1
        };
        var collider = hitboxes[comboSeq].GetComponent<Collider>();
        Renderer renderer = hitboxes[comboSeq].GetComponent<Renderer>();

        if (!attackFirst)
        {
            var color = renderer.material.color;
            color.a = 0.4f;
            renderer.material.color = color;
            if (collider) collider.enabled = false;
            
            yield return new WaitForSeconds(waitTime);
            
            if (collider) collider.enabled = true;
            color.a = 1f;
            renderer.material.color = color;
        }
        else
        {
            var color = renderer.material.color;
            color.a = 1f;
            renderer.material.color = color;
            if (collider) collider.enabled = true;
            
            yield return new WaitForSeconds(waitTime);
            
            if (collider) collider.enabled = false;
            color.a = 0.4f;
            renderer.material.color = color;
        }
        
    }
}