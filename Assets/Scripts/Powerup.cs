using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.XR;
//using static UnityEditor.ShaderData;


public class Powerup : MonoBehaviour
{
   
    [SerializeField]
    private UIManager _UIManager;
    [SerializeField]
    private float _speed = 4.0f;
    [SerializeField]
    private int _powerUpID;
    [SerializeField]
    private AudioClip _collectedSoundEffect;
    [SerializeField]
    private GameObject _camera;
    [SerializeField]
    private GameObject _explosion;



    [SerializeField]
    private int _chance;






    [SerializeField]
    private bool _isCalledTowardsPlayer;
    [SerializeField]
    private float _calledSpeed;
    [SerializeField]
    private bool _canEffect = true;
    [SerializeField]
    private float _yPositionForDeath;



    // Start is called before the first frame update
    void Start()
    {
        _UIManager = FindObjectOfType<UIManager>();
        _camera = GameObject.FindWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        if (_isCalledTowardsPlayer)
        {
            transform.position = Vector3.MoveTowards(transform.position, GameObject.FindWithTag("Player").transform.position, _calledSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
            if (transform.position.y < _yPositionForDeath)
            {
                Destroy(this.gameObject);
            }
        }

        
    }

    public int GetChance()
    {
        return _chance;
    }






    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            if (player != null && _canEffect)
            {
                switch (_powerUpID) {
                    case 0:
                        player.CallTripleShot();
                        break;
                    case 1:
                        player.CallSpeedBoost();
                        break;
                    case 2:
                        player.CallShield();
                        break;
                    case 3:
                        player.ReviveCollected();
                        break;
                    case 4:
                        player.AmmoChanged(5);
                        break;
                    case 5:
                        player.ImmortalCollected();
                        break;
                    case 6:
                        player.Damage(1);
                        break;
                    case 7:
                        player.HeatSeekingMissileCollected();
                        break;
                }
                if (_powerUpID != 6)
                {
                    _UIManager.PowerUpCollected();
                    AudioSource.PlayClipAtPoint(_collectedSoundEffect, _camera.transform.position, 90f);
                    Destroy(this.gameObject);
                }
                else
                {
                    BeDestroyed();
                }
               
            }
        }
        else if(other.tag == "Laser")
        {
            Laser laser = other.GetComponent<Laser>();
            if (laser.CanDestroyPowerUp())
            {
                Destroy(other.gameObject);
                BeDestroyed();
            }
        }

    }

    public void Called()
    {
        _isCalledTowardsPlayer = true;
    }


    public void BeDestroyed()
    {
        _canEffect = false;
        _speed = 0;
        _calledSpeed = 0;
        Instantiate(_explosion, transform.position, Quaternion.identity);
        Destroy(this.gameObject, .25f);

    }



}