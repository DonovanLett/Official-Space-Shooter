using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private SpawnManager _spawnManager;
    [SerializeField]
    private bool _notDestroyed = true;
    [SerializeField]
    private GameObject _explosion;
    [SerializeField]
    private GameObject _camera;
    [SerializeField]
    private UIManager _uiManager;
    [SerializeField]
    private Timer _timer;
    // Start is called before the first frame update
    void Start()
    {
        _camera = GameObject.FindWithTag("MainCamera");
        _spawnManager = FindObjectOfType<SpawnManager>();
        _uiManager = FindObjectOfType<UIManager>();
        _timer = FindObjectOfType<Timer>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, 0f, 0.1f, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Laser" && _notDestroyed == true)
        {
            _notDestroyed = false;
            Destroy(other.gameObject);
            _spawnManager.OnRoundStart();
            Instantiate(_explosion, transform.position, Quaternion.identity);
            _timer.StartTimer();
            _uiManager.ShowOnScreenUI();
            Destroy(this.gameObject, 0.5f);
        }
    }
}