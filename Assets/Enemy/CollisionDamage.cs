using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDamage : MonoBehaviour
{
    [SerializeField]
    private int damage = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.tag.Equals("EnemyAttack") && other.gameObject.tag.Equals("Player"))
        {
            other.GetComponent<Health>().TakeDamage(damage);

            Vector3 targetDir = other.transform.position - transform.position;
            Vector3 knockbackDir = new Vector3(targetDir.x, 0, targetDir.z);
            // other.GetComponent<CharacterControls>().ApplyKnockback(knockbackDir, 10);
        }

        if (gameObject.tag.Equals("PlayerAttack") && other.gameObject.tag.Equals("Enemy"))
        {
            other.GetComponent<Health>().TakeDamage(damage);

            Vector3 targetDir = other.transform.position - transform.position;
            Vector3 knockbackDir = new Vector3(targetDir.x, 0, targetDir.z);
            // other.GetComponent<CharacterControls>().ApplyKnockback(knockbackDir, 10);
            
            // Debug.Log("Hit enemy");
        }

    }
}
