using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

    [SerializeField]
    private Transform _camTransform;
    [SerializeField]
    private float _shakeDuration = 0f;
    [SerializeField]
    private float _shakeAmount = 0.01f;
    [SerializeField]
    private float _decreaseFactor = 1.0f;
    [SerializeField]
    private Vector3 _originalPos;

    void Awake()
    {
        if (_camTransform == null)
        {
            _camTransform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    public void StartShake(float secondsOfShaking, float shakeIntensity)
    {
        _shakeDuration = secondsOfShaking;
        _shakeAmount = shakeIntensity;
    }

    void OnEnable()
    {
        _originalPos = _camTransform.localPosition;
    }

    void Update()
    {
        if (_shakeDuration > 0)
        {
            _camTransform.localPosition = _originalPos + Random.insideUnitSphere * _shakeAmount;
            _shakeDuration -= Time.deltaTime * _decreaseFactor;
        }
        else
        {
            _shakeDuration = 0f;
            _camTransform.localPosition = _originalPos;
        }
    }














    // Start is called before the first frame update
    void Start()
    {
        
    }

    
}
