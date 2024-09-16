using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    [Header("Components")] 
    public Metronome metronome;
    public MusicManager musicManager;
    public EnemyController[] EnemyControllers;

    private void Start()
    {
        EnemyControllers = FindObjectsOfType<EnemyController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.tag.Equals("Player")) return;

        if (metronome) metronome.isTriggerActive = true;
        if (musicManager) musicManager.isTriggerActive = true;

        if (EnemyControllers != null)
        {
            foreach (var enemy in EnemyControllers)
            {
                enemy.isTriggerActive = true;
            }
        }
    }
}
