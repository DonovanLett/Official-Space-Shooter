using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5.0f;
    [SerializeField]
    private bool _isEnemyLaser = false;
    [SerializeField]
    private AudioClip _zapSoundEffect;
    [SerializeField]
    private int _laserID;
    [SerializeField]
    private GameObject _camera;
    [SerializeField]
    private int _laserDamage;
    [SerializeField]
    private bool _canDestroyPowerUp;
    [SerializeField]
    private bool _isMovingDown = false;
    [SerializeField]
    private float _xLimit, _yLimit;
    
    private GameObject _targetObject;

    // Start is called before the first frame update
    void Start()
    {
        _camera = GameObject.FindWithTag("MainCamera");
        if (_zapSoundEffect != null)
        {
            AudioSource.PlayClipAtPoint(_zapSoundEffect, _camera.transform.position, 90f);
        }
    }

    public void SetVariables(bool _isEnemy, bool _isDown, bool _destroysPowerUp)
    {
        _isEnemyLaser = _isEnemy;
        _isMovingDown = _isDown;
        _canDestroyPowerUp = _destroysPowerUp;
    }

    public bool IsEnemyLaser()
    {
        return _isEnemyLaser;
    }
    

    // Update is called once per frame
    void Update()
    {
        if (_laserID == 0)
        {
            if (_isMovingDown)
            {
                MoveDown();
            }
            else
            {
                MoveUp();
                SignalDodger();
            }
            SetToDestroy();
        }
    }

    private void FixedUpdate()
    {
        if (_laserID == 1)
        {
            MoveToNearestTarget();
            SetToDestroy();
        }
    }

    public void MoveUp()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
    }

    public void MoveDown()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
    }

    public void MoveToNearestTarget()
    {
        GameObject[] _enemiesInScene = GameObject.FindGameObjectsWithTag("Enemy");
        List<GameObject> _aliveEnemies = new List<GameObject>();

        for(int i = 0; i < _enemiesInScene.Length; i++)
        {
            if (_enemiesInScene[i].GetComponent<Boss>() != null && _enemiesInScene[i].GetComponent<Boss>().CanBeHurt() == true)
            {
                _aliveEnemies.Add(_enemiesInScene[i]);
                break;
                
            }
            else if (_enemiesInScene[i].GetComponent<Enemy>() != null && _enemiesInScene[i].GetComponent<Enemy>().IsAlive() == true)
            {
                _aliveEnemies.Add(_enemiesInScene[i]);
            }
        }

        if (_aliveEnemies.Count > 0)
        {
            GameObject nearestEnemy = _aliveEnemies[0];
          //  transform.Translate(Vector3.up * _speed * Time.deltaTime);

            float distanceToNearest = Vector3.Distance(transform.position, nearestEnemy.transform.position);
            for (int i = 1; i < _aliveEnemies.Count; i++)
            {
                float distanceToCurrent = Vector3.Distance(transform.position, _aliveEnemies[i].transform.position);
                if (distanceToCurrent < distanceToNearest)
                {
                    nearestEnemy = _aliveEnemies[i];
                    distanceToNearest = distanceToCurrent;
                }
            }

            Vector3 targetLocation = nearestEnemy.transform.position;

            if (nearestEnemy.GetComponent<Boss>() != null)
            {
                targetLocation += new Vector3(0, -10.35f, 0);
            }
            if (nearestEnemy != null)
             {
                 Vector2 direction = targetLocation - transform.position;
                 float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                 Quaternion targetRotation = Quaternion.Euler(0, 0, angle - 90);
                
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 7);
              }
            transform.position = Vector3.MoveTowards(transform.position, targetLocation, _speed * Time.deltaTime);
        }
        else
        {
            MoveUp();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
            if (other.tag == "Player" && _isEnemyLaser)
            {
                Player player = other.GetComponent<Player>();
                if (player != null)
                {
                player.Damage(_laserDamage);
                Destroy(this.gameObject);
            }
            }
    }


    public bool CanDestroyPowerUp()
    {
        return _canDestroyPowerUp;
    }

    public void SignalDodger()
    {
        int _enemyMask = LayerMask.GetMask("Dodger");
        Vector2 origin = transform.position;
        Vector2 direction = Vector2.up;
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, 100f, _enemyMask);
        if (hit.collider != null && hit.collider.CompareTag("Enemy"))
        {
            hit.collider.GetComponent<Enemy>().Dodge();
        }
    }

    public void SetToDestroy()
    {
        if (transform.position.y > _yLimit || transform.position.y < -_yLimit || transform.position.x > _xLimit || transform.position.x < -_xLimit)
        {
            Destroy(this.gameObject);
        }
    }
}