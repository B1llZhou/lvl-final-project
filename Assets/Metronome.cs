using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metronome : MonoBehaviour {
    public MusicManager musicManager;
    public GameObject sphere;
    public float twinkleDuration = 0.2f;

    // Update is called once per frame
    void Update()
    {
        if (musicManager.IsOnBeat()) {
            StartCoroutine(Twinkle());
        }
    }

    private IEnumerator Twinkle() {
        sphere.SetActive(true);
        yield return new WaitForSeconds(twinkleDuration);
        sphere.SetActive(false);
    }
}
