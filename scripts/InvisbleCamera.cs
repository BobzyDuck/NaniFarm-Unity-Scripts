using UnityEngine;

public class InvisbleCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Camera playerCamera;

    [Header("Layer Selection")]
    [SerializeField] private LayerMask visibleLayersInTrigger;

    [Header("Player Detection")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private Transform centerPoint;
    [SerializeField] private bool usePlayerCenter = true;

    [Header("Tab Apple")]
    [SerializeField] private GameObject tabApple;

    private Collider triggerCollider;
    private int defaultCullingMask;
    private bool playerFullyInside = false;
    private GameObject playerObject = null;

    private void Start()
    {
        triggerCollider = GetComponent<Collider>();

        if (playerCamera == null)
            playerCamera = Camera.main;

        if (playerCamera != null)
            defaultCullingMask = playerCamera.cullingMask;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && playerObject == null)
        {
            playerObject = other.gameObject;

            if (centerPoint == null && usePlayerCenter)
                centerPoint = playerObject.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag) && other.gameObject == playerObject)
        {
            // خروج طبق سیستم یونیتی → ماسک فوراً برگردد
            OnPlayerFullyExit();

            playerObject = null;
            playerFullyInside = false;

            if (usePlayerCenter)
                centerPoint = null;
        }
    }

    private void Update()
    {
        // فقط ورود با مرکز کنترل می‌شود
        if (playerObject != null && centerPoint != null && !playerFullyInside)
        {
            bool isCenterInside = triggerCollider.bounds.Contains(centerPoint.position);

            if (isCenterInside)
            {
                OnPlayerFullyEnter();
                playerFullyInside = true;
            }
        }
    }

    private void OnPlayerFullyEnter()
    {
        if (playerCamera != null)
        {
            playerCamera.cullingMask = visibleLayersInTrigger.value;

            if(tabApple != null)
            tabApple.gameObject.SetActive(false);
        }
    }

    private void OnPlayerFullyExit()
    {
        if (playerCamera != null)
        {
            playerCamera.cullingMask = defaultCullingMask;

            if (tabApple != null)
                tabApple.gameObject.SetActive(true);
        }
    }
}
