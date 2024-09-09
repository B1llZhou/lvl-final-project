using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int hp = 1;
    [HideInInspector]
    public int maxhp;

    [SerializeField]
    private float iTime = 1f;
    private float iTimer = 2f;

    [SerializeField]
    private Material defaultMaterial;
    [SerializeField]
    private Material damageMaterial;

    [HideInInspector]
    public bool canTakeDamage = true;

    [SerializeField]
    private bool canDie = true;

    void Start()
    {
        maxhp = hp;
    }

    void Update()
    {
        if (iTimer < iTime)
        {
            iTimer += Time.deltaTime;
        }
    }

    public void TakeDamage(int damage)
    {
        if (canTakeDamage && iTimer > iTime)
        {
            iTimer = 0f;
            Debug.Log($"{gameObject.name} took {damage} damage");
            hp -= damage;

            //Insert die function
            if (canDie && hp < 0)
            {
                Destroy(gameObject);
            }

            showDamage();
        }
    }

    private void showDamage()
    {
        setDamageMaterial();
        Invoke(nameof(setRegularMaterial), .1f);
        Invoke(nameof(setDamageMaterial), .2f);
        Invoke(nameof(setRegularMaterial), .3f);
    }

    private void setDamageMaterial()
    {
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material = damageMaterial;
    }

    private void setRegularMaterial()
    {
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material = defaultMaterial;
    }
}
