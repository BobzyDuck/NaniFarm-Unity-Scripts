using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTransitionZone : MonoBehaviour
{
    private CameraManager cameraManager;

    [Header("FOV Duration Settings")]
    [SerializeField] private float fovDurationEnter = 2.0f; 
    [SerializeField] private float fovDurationExit = 4.0f; 

    [Header("Rotation Settings")]
    [SerializeField] private bool enableRotation = false;
    [SerializeField] private float targetRotationEnter = 45.0f;

    [SerializeField] private float _targetFOV;

    private void Start()
    {
        cameraManager = CameraManager.Instance;
        if (cameraManager == null)
        {
            Debug.LogError("CameraManager not found in the scene.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && cameraManager != null)
        {
            cameraManager.ChangeFOV(_targetFOV, true, fovDurationEnter); 

            if (enableRotation)
            {
                cameraManager.ChangeCameraRotation(targetRotationEnter, fovDurationEnter); 
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && cameraManager != null)
        {
            cameraManager.ChangeFOV(cameraManager.GetOriginalFOV(), false, fovDurationExit);

            if (enableRotation)
            {
                cameraManager.ChangeCameraRotation(cameraManager.GetOriginalRotation(), fovDurationExit);
            }
        }
    }
}