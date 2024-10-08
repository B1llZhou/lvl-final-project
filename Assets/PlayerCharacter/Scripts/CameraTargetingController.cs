using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraTargetingController : MonoBehaviour {
    [Header("Target System Basic")]
    [SerializeField] private Targetable[] allTargetables;
    [SerializeField] private Targetable currentTarget;
    [SerializeField] public float targetRange = 5f;
    [SerializeField] public float rotationSpeed = 10f;
    [SerializeField] private bool isFocus;

    [Header("Vcam Settings")] 
    [SerializeField] private CinemachineBrain brain;
    private CinemachineVirtualCamera currentVcam;
    private GameObject followCenter;
    
    
    private void Start() {
        allTargetables = FindObjectsOfType<Targetable>();
        followCenter = new GameObject();
    }

    private void Update()
    {
        if (brain) currentVcam = brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();

        currentTarget = FindNearestTargetable();
        if (currentTarget == null) return;
        
        if (isFocus)
        {
            RotateToTarget(currentTarget);
        }
        
        foreach (var t in allTargetables) {
            if (isFocus && t == currentTarget) t.OnTargeted();
            else t.OnTargetLost();
        }
    }

    private void RotateToTarget(Targetable target) {
        if (currentTarget == null) return;
        
        Vector3 direction = (target.transform.position - transform.position).normalized;
        direction.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }
    
    private Targetable FindNearestTargetable()
    {
        Targetable nearestTarget = null;
        float shortestDistance = Mathf.Infinity;  // Start with the largest possible distance
        Vector3 currentPosition = transform.position;

        if (allTargetables == null || allTargetables.Length == 0) return null;
        foreach (var target in allTargetables)
        {
            if (target == null) continue;
            // Check if the target is valid and within the specified range
            if (target.IsTargetable())
            {
                float distanceToTarget = Vector3.Distance(currentPosition, target.transform.position);

                if (distanceToTarget <= targetRange && distanceToTarget < shortestDistance)
                {
                    shortestDistance = distanceToTarget;
                    nearestTarget = target;
                }
            }
        }

        return nearestTarget;  // Return the nearest targetable object, or null if none are found
    }
    
    public void OnFocus(InputAction.CallbackContext context) {
        if (context.performed) {
            isFocus = true;
        }
        else if (context.canceled) {
            isFocus = false;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        if (currentTarget == null) return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, currentTarget.transform.position);
    }
#endif
}
