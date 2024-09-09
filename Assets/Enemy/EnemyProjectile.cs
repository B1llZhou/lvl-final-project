using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField]
    private float speed = 5;
    [SerializeField]
    private float lifetime = 5;
    private float curLife = 0;

    // Update is called once per frame
    void Update()
    {
        curLife += Time.deltaTime;
        if (curLife >= lifetime)
        {
            Destroy(gameObject);
        }
        transform.position += transform.forward * Time.deltaTime * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player") || other.gameObject.tag.Equals("PlayerAttack"))
        {
            Destroy(gameObject);
        }
    }
}
