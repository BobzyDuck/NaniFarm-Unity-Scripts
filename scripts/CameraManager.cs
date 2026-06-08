using Unity.Cinemachine;
using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [SerializeField] public Camera _mainCamera;
    [SerializeField] public CinemachineCamera _firstCamera; 

    private float _originalFOV;
    private float _enteredFOV; 
    private Coroutine _fovCoroutine;
    private Coroutine _rotationCoroutine;

    private float _originalRotation;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected virtual void Start()
    {
        _originalFOV = _firstCamera.Lens.FieldOfView; 
        _originalRotation = _firstCamera.transform.rotation.eulerAngles.y; 
    }

    public float GetOriginalRotation()
    {
        return _originalRotation;
    }

    public float GetOriginalFOV()
    {
        return _originalFOV;
    }

    public void ChangeFOV(float newFOV, bool isEntering, float duration)
    {
        if (_fovCoroutine != null)
        {
            StopCoroutine(_fovCoroutine);
        }

        if (isEntering)
        {
            _enteredFOV = _firstCamera.Lens.FieldOfView;
        }

        _fovCoroutine = StartCoroutine(ChangeFOVCoroutine(newFOV, isEntering, duration));
    }

    private IEnumerator ChangeFOVCoroutine(float newFOV, bool isEntering, float duration)
    {
        float elapsedTime = 0f;
        float startingFOV = isEntering ? _enteredFOV : _firstCamera.Lens.FieldOfView; 

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            t = Mathf.SmoothStep(0f, 1f, t);

            _firstCamera.Lens.FieldOfView = Mathf.Lerp(startingFOV, newFOV, t); 
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _firstCamera.Lens.FieldOfView = newFOV; 
        _fovCoroutine = null;
    }

    public void ChangeCameraRotation(float targetRotation, float duration)
    {
        if (_rotationCoroutine != null)
        {
            StopCoroutine(_rotationCoroutine);
        }

        _rotationCoroutine = StartCoroutine(ChangeRotationCoroutine(targetRotation, duration));
    }

    private IEnumerator ChangeRotationCoroutine(float targetRotation, float duration)
    {
        float elapsedTime = 0f;
        float startingRotation = _firstCamera.transform.rotation.eulerAngles.y;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            t = Mathf.SmoothStep(0f, 1f, t);

            float currentRotation = Mathf.LerpAngle(startingRotation, targetRotation, t);
            Vector3 currentEulerAngles = _firstCamera.transform.rotation.eulerAngles;
            _firstCamera.transform.rotation = Quaternion.Euler(currentEulerAngles.x, currentRotation, currentEulerAngles.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Vector3 finalEulerAngles = _firstCamera.transform.rotation.eulerAngles;
        _firstCamera.transform.rotation = Quaternion.Euler(finalEulerAngles.x, targetRotation, finalEulerAngles.z);

        _rotationCoroutine = null;
    }

    public void SetCameraInstantly(float newFOV, float newRotation)
    {
        _firstCamera.Lens.FieldOfView = newFOV;  
        Vector3 currentEulerAngles = _firstCamera.transform.rotation.eulerAngles;
        _firstCamera.transform.rotation = Quaternion.Euler(currentEulerAngles.x, newRotation, currentEulerAngles.z);
    }
}
