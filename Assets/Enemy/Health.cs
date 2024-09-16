using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Components")] 
    [SerializeField] private Text healthText;
    // [SerializeField] private TextMesh healthTextMesh;
    [SerializeField] private TextMeshPro healthTMP;
    
    [Header("Health stats")]
    public int hp = 1;
    [HideInInspector] public int maxhp;

    [SerializeField] private float iTime = 1f;
    private float iTimer = 2f;

    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material damageMaterial;

    [HideInInspector] public bool canTakeDamage = true;

    [SerializeField] private bool canDie = true;

    private void Start()
    {
        maxhp = hp;
    }

    private void Update()
    {
        if (iTimer < iTime)
        {
            iTimer += Time.deltaTime;
        }
        ShowHealth();
    }

    public void TakeDamage(int damage)
    {
        if (canTakeDamage && iTimer > iTime)
        {
            iTimer = 0f;
            hp -= damage;
            Debug.Log($"{gameObject.name} took {damage} damage; HP: {hp}");

            //Insert die function
            if (canDie && hp <= 0)
            {
                if (gameObject.tag.Equals("Player")) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                else Destroy(gameObject);
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
        MeshRenderer meshRenderer = gameObject.GetComponentInChildren<MeshRenderer>();
        if (meshRenderer == null) return;
        meshRenderer.material = damageMaterial;
    }

    private void setRegularMaterial()
    {
        MeshRenderer meshRenderer = gameObject.GetComponentInChildren<MeshRenderer>();
        if (meshRenderer == null) return;
        meshRenderer.material = defaultMaterial;
    }

    private void ShowHealth()
    {
        if (healthText != null) healthText.text = $"HP: {hp}";
        if (healthTMP != null)
        {
            healthTMP.text = "HP: " + hp.ToString();
            var lookRotation = healthTMP.transform.position - Camera.main.transform.position;
            lookRotation.y = 0;
            healthTMP.transform.rotation = Quaternion.LookRotation(lookRotation);
        }
    }
}
