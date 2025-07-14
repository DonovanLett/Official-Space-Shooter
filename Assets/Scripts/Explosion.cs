using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField]
    private AudioClip _explosionSoundEffect;
    [SerializeField]
    private GameObject _mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        _mainCamera = GameObject.FindWithTag("MainCamera");
        AudioSource.PlayClipAtPoint(_explosionSoundEffect, _mainCamera.transform.position, 90f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AnimationFinished()
    {
        Destroy(this.gameObject);
    }
}
