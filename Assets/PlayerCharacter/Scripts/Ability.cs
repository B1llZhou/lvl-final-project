using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    public GameObject model;
    public float existTime = 0.5f;
    public List<AudioClip> audioClips = new List<AudioClip>();
    public AudioSource audioSource;
    private Coroutine releaseCo;

    private void Start()
    {
        model.gameObject.SetActive(false);
    }

    public bool Release(int chord = 0) {
        if (releaseCo != null) {
            //SetAbilityOn(false);
            //releaseCo = null;   
            return false;
        }

        releaseCo = StartCoroutine(ReleaseCo(chord));

        return true;
    }

    private IEnumerator ReleaseCo(int chord = 0) {
        SetAbilityOn(true, chord);
        yield return new WaitForSeconds(existTime);
        SetAbilityOn(false);
        releaseCo = null;
    }

    private void SetAbilityOn(bool b, int chord = 0) {
        if (model != null) {
            model.gameObject.SetActive(b);
        }

        if (b && audioSource != null) {
            if (chord < audioClips.Count) {
                audioSource.PlayOneShot(audioClips[chord]);
            } else {
                audioSource.PlayOneShot(audioClips[audioClips.Count-1]);
            }
        }
    }
}