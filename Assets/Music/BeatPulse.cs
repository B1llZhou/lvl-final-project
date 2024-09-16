using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BeatPulse : MonoBehaviour
{
    private UnityAction beatAction;
    [SerializeField]
    private float beatSubdivision = 1;
    [SerializeField]
    private float beatScale = 1.3f;
    private Vector3 baseScale;
    [SerializeField]
    private float scaleDownTime = .1f;

    // Start is called before the first frame update
    void Start()
    {
        beatAction += StartBeatPulse;
        baseScale = transform.localScale;
    }

    public void Play()
    {
        // BeatManager.GetInstance().RegisterBeatFunction(beatAction, beatSubdivision);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localScale.magnitude > baseScale.magnitude)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, baseScale, scaleDownTime);
        }
        else
        {
            transform.localScale = Vector3.one;
        }
        
    }

    public void StartBeatPulse()
    {
        transform.localScale *= beatScale;
    }
}