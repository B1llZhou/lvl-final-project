using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetEnemy : MonoBehaviour {
    public GameObject[] enemy;
    // Start is called before the first frame update
    void Start()
    {
        enemy ??= GameObject.FindGameObjectsWithTag("Enemy");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
