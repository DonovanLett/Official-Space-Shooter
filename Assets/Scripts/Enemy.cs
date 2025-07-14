using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;
    [SerializeField]
    private UIManager _UIManager;
    [SerializeField]
    Animator _animator;
    [SerializeField]
    private bool _isAlive;
    [SerializeField]
    private int _enemyID;
    [SerializeField]
    private AudioClip _explosionSoundEffect;
    [SerializeField]
    private GameObject _camera;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private bool _isPlayerAlive = true;
    [SerializeField]
    private int _pointsTheyAreWorth;
    [SerializeField]
    private GameObject _turretOne, _turretTwo;
    [SerializeField]
    private float _timeBetweenBullets;
    [SerializeField]
    private float _minShotPause;
    [SerializeField]
    private float _maxShotPause;
    [SerializeField]
    private float _minStartPause;
    [SerializeField]
    private float _maxStartPause;
    [SerializeField]
    private int _chance;
    [SerializeField]
    private float _minXPosition;
    [SerializeField]
    private float _maxXPosition;
    [SerializeField]
    private float _yPosition;
    [SerializeField]
    private float _yPositionForDeath;
    [SerializeField]
    private SpriteRenderer _spriteRenderer;
    [SerializeField]
    private Color _originalColor;
    [SerializeField]
    private int _damageUponPlayerImpact;
    

    [Header("SPINNER")]
    [SerializeField]
    private float _spinningSpeed;

    [Header("JUMPY")]
    [SerializeField]
    private float _sideSpeed;

    [Header("BRUTE")]
    [SerializeField]
    private int _health;

    [Header("BANZAI")]
    [SerializeField]
    private float _waitingSpeed;
    [SerializeField]
    private float _rammingSpeed;
    [SerializeField]
    private bool _isRamming;

    [Header("SMARTY-PANTS")]
    [SerializeField]
    private bool _canDodge;
    [SerializeField]
    private float _dodgeSpeed;
    [SerializeField]
    private float _dodgeLength;
    [SerializeField]
    private bool _isDodging;
    [SerializeField]
    private GameObject _backTurret;
    [SerializeField]
    private GameObject _powerUpTurret;
    [SerializeField]
    private bool _canFireBackwards;
    [SerializeField]
    private bool _canFireAtPowerUp;



    //  [Header("DODGER")]


    // Start is called before the first frame update
    void Start()
    {
        _camera = GameObject.FindWithTag("MainCamera");
        _UIManager = FindObjectOfType<UIManager>();
        _animator = gameObject.GetComponent<Animator>();
        _isAlive = true;
        _originalColor = _spriteRenderer.color;
        if (_enemyID != 4)
        {
            StartCoroutine(EnemyFire());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_isAlive)
        {
            switch (_enemyID) {
                default:
                    transform.Translate(Vector3.down * _speed * Time.deltaTime);
                    break;
                case 1:
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y - 50, transform.position.z), _speed * Time.deltaTime);
                    transform.Rotate(0f, 0f, _spinningSpeed, Space.Self);
                    break;
                case 2:
                    transform.Translate(Vector3.down * _speed * Time.deltaTime);
                    transform.Translate(Vector3.right * _sideSpeed * Time.deltaTime);
                    if (transform.position.x >= 9.626f)
                    {
                        _sideSpeed = Mathf.Abs(_sideSpeed) * -1;
                    }
                    else if (transform.position.x <= -9.626f)
                    {
                        _sideSpeed = Mathf.Abs(_sideSpeed);
                    }
                    
                    break;
                case 5:
                    transform.Translate(Vector3.down * _speed * Time.deltaTime);
                    if (_isDodging)
                    {
                        transform.Translate(Vector3.right * _dodgeSpeed * Time.deltaTime);
                    }
                    break;
            }
            if (transform.position.y < _yPositionForDeath)
            {
                Destroy(this.gameObject);
            }

            int _playerMask = LayerMask.GetMask("Player");
            int _powerUpMask = LayerMask.GetMask("PowerUp");
            Vector2 origin = transform.position;
            if(_enemyID == 5 && _canFireBackwards)
            {
                if (_canFireBackwards)
                {
                    Vector2 upDirection = Vector2.up;
                    RaycastHit2D hit = Physics2D.Raycast(origin, upDirection, 100f, _playerMask);
                    if (hit.collider != null && hit.collider.CompareTag("Player"))
                    {
                        if (_backTurret != null)
                        {
                            _backTurret.GetComponent<Turret>().Fire();
                            _canFireBackwards = false;
                        }
                    }
                }

                if (_canFireAtPowerUp)
                {
                    Vector2 downDirection = Vector2.down;
                    RaycastHit2D hit = Physics2D.Raycast(origin, downDirection, 100f, _powerUpMask);
                    if (hit.collider != null && hit.collider.CompareTag("PowerUp"))
                    {
                        if (_powerUpTurret != null)
                        {
                            _powerUpTurret.GetComponent<Turret>().Fire();
                            _canFireAtPowerUp = false;
                        }
                    }
                }
            }


            if (_enemyID == 4)
            {
               // int _playerMask = LayerMask.GetMask("Player");
               //   Vector2 origin = transform.position;
                BanzaiSpeedController();
                Vector2 downDirection = Vector2.down;
                RaycastHit2D banzaiHit = Physics2D.Raycast(origin, downDirection, 100f, _playerMask);
                if (banzaiHit.collider != null && banzaiHit.collider.CompareTag("Player"))
                {
                    _isRamming = true;
                }
            }
        }
     }

    public bool IsAlive()
    {
        return _isAlive;
    }



    public int GetChance()
    {
        return _chance;
    }

    public Vector3 GetStart()
    {
        return new Vector3(Random.Range(_minXPosition, _maxXPosition), _yPosition, 0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isAlive)
        {
            if (other.tag == "Player")
            {
                Player player = other.GetComponent<Player>();
                if (player != null)
                {
                   player.Damage(_damageUponPlayerImpact);
                    Die();
                }
            }
            else if (other.tag == "Laser" && other.GetComponent<Laser>().IsEnemyLaser() == false)
            {
                Destroy(other.gameObject);
                if (_enemyID == 3)
                {
                    _health--;
                    StopCoroutine(Damage());
                    StartCoroutine(Damage());
                    if(_health <= 0)
                    {
                        _UIManager.PointsAdded(_pointsTheyAreWorth);
                        Die();
                    }
                }
                else
                {
                    _UIManager.PointsAdded(_pointsTheyAreWorth);
                    Die();
                }
            }
        }
    }

    IEnumerator Damage()
    {
        _spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        _spriteRenderer.color = _originalColor;
    }

    IEnumerator EnemyFire()
    {
        yield return new WaitForSeconds(Random.Range(_minStartPause, _maxStartPause));

        while (_isAlive && _isPlayerAlive)
        {
            _turretOne.GetComponent<Turret>().Fire();
            if (_enemyID == 2) {
                _sideSpeed *= -1;
            }
            if(_enemyID == 2) { 
            yield return new WaitForSeconds(Random.Range(_minShotPause, _maxShotPause)); //0.75
            }
            else
            {
                yield return new WaitForSeconds(_timeBetweenBullets); //0.75
            }
            if (_isAlive && _isPlayerAlive)
            {
                _turretTwo.GetComponent<Turret>().Fire();
                if (_enemyID == 2)
                {
                    _sideSpeed *= -1;
                }
            }
            yield return new WaitForSeconds(Random.Range(_minShotPause, _maxShotPause)); //3 and 7
        }
    }

    private void Die()
    {
        _UIManager.EnemyKilled();
        _animator.SetTrigger("OnEnemyDeath");
        AudioSource.PlayClipAtPoint(_explosionSoundEffect, _camera.transform.position, 90f);
        _isAlive = false;
        Destroy(this.gameObject, 3.8f);
    }

    public void BanzaiSpeedController()
    {
        if (!(_isRamming))
        {
            if(_speed > _waitingSpeed)
            {
                _speed -= 0.01f;
            }
        }
        else if (_isRamming)
        {
            if(_speed < _rammingSpeed)
            {
                _speed++;
            }
        }
    }

    public void Dodge()
    {
        if(_enemyID == 5 && _canDodge)
        {
            bool goesLeft = ((int)Random.Range(0, 2)) == 0;
            if (transform.position.x > 7)
            {
                _dodgeSpeed *= -1;
            }
            else if(transform.position.x > -7)
            {
                if (goesLeft)
                {
                    _dodgeSpeed *= -1;
                }
            }
                StartCoroutine(DodgeRoutine());
               _canDodge = false;
        }
    }

    IEnumerator DodgeRoutine()
    {
        _isDodging = true;
        yield return new WaitForSeconds(_dodgeLength);
        _isDodging = false;
    }


    public void OnPlayerDeath()
    {
        Debug.Log("Should Be Stopped");
        _isPlayerAlive = false;
        StopCoroutine(EnemyFire());
    }
}