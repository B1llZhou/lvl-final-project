using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    public EnemyController[] EnemyControllers;

    private void Start()
    {
        EnemyControllers = FindObjectsOfType<EnemyController>();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.tag.Equals("Player")) return;
        
        if (EnemyControllers != null)
        {
            foreach (var enemy in EnemyControllers)
            {
                enemy.enemyCanMove = true;
            }
        }
    }
}
