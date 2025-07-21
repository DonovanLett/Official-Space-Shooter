using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
//using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.Playables;

public class Player : MonoBehaviour
{

    [Header("Borders")]
    [SerializeField]
    private float _topBorder;
    [SerializeField]
    private float _bottomBorder;
    [SerializeField]
    private float _sideBorders;
    
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _normalLaserPrefab;
    [SerializeField]
    private GameObject _missilePrefab;
    [SerializeField]
    private float _speed = 1f;    // What the Players speed is currently
    [SerializeField]
    private float _normalSpeed;   // What the Players speed is when the Thruster is being used
    [SerializeField]
    private float _thrusterSpeed; // What the Players speed is initially and what is should revert to upon the Thruster running out
    [SerializeField]
    private float _powerUpSpeedBoost;
    [SerializeField]
    private bool _isTripleShotActive = false;
    [SerializeField]
    private int _lives = 5;

    [Header("Fire Information")]
    [SerializeField]
    private float _tripleLaserXOffSet = 0.786f;
    [SerializeField]
    private float _tripleLaserYOffSet = 0.498f;
    [SerializeField]
    private float _canFire = -1.0f;
    [SerializeField]
    private float _fireRate = 0.5f;
    [SerializeField]
    private int _points = 0;
    [SerializeField]
    private SpawnManager _spawnManagerScript;
    [SerializeField]
    private UIManager _uiManager;
    [SerializeField]
    private float _fireOffset = 0.75f;
    [SerializeField]
    private PlayableDirector _gameOverTimeline;
    [SerializeField]
    private GameObject _shieldObject;
    [SerializeField]
    private int _shieldStrength = 0;
    [SerializeField]
    private AudioSource _lasers;
    [SerializeField]
    private GameObject _leftFireBall, _rightFireBall;
    [SerializeField]
    private int _ammo; //Our current amount of Ammo.
    [SerializeField]
    private int _maxAmmo; // The Maximum amount of Ammo we are allowed to have.
    [SerializeField]
    private GameObject _mainCamera;
    [SerializeField]
    private CameraShake _cameraShake;
    [SerializeField]
    private float _thrusterPower = 20.0f;    //The Amount of Power the Thruster currently has
    [SerializeField]
    private float _thrusterPowerMax = 20.0f; //The Amount of Power the Thruster is allowed to have
    [SerializeField]
    private bool _canUseThruster = true;     // Whether or not the Thruster can be used or is refilling
    [SerializeField]
    private float _thrusterDrainedPerFrame;  // How much power the Thruster drains by when being used (per frame)
    [SerializeField]
    private float _thrusterRefillPerFrame;   // How much power the Thruster refills by when recharging (per frame)
    [SerializeField]
    private ShieldDamageEffect _damageEffect;
    [SerializeField]
    private bool _isImmortal = false;
    [SerializeField]
    private float _immortalLaserAngle = 25.0f;
    [SerializeField]
    private Color _originalColor;
    [SerializeField]
    private GameObject _explosion;
    [SerializeField]
    private bool _canMove = false;
    [SerializeField]
    private AudioController _mainAudio;
    [SerializeField]
    private SpriteRenderer _spriteRenderer;
    [SerializeField]
    private Timer _timer;


    [SerializeField]
    private int _powerUpCaller;
    [SerializeField]
    private int _numOfCallsPerRound = 2;
    [SerializeField]
    private int _roundItStartsCalling;

    [Header("Power Ups Lasting Times")]
    [SerializeField]
    private float _tripleShot;
    [SerializeField]
    private float _speedBoost;
    [SerializeField]
    private float _immortality;
    [SerializeField]
    private float _heatSeekingMissile;





    // Start is called before the first frame update
    void Start()
    {
        _laserPrefab = _normalLaserPrefab;
        transform.position = new Vector3(0f, -2f, -0.1f);
        _spawnManagerScript = FindObjectOfType<SpawnManager>();
        _uiManager = FindObjectOfType<UIManager>();
        _shieldObject.SetActive(false);
        _ammo = _maxAmmo;
        _uiManager.AmmoUpdated(_ammo, _maxAmmo);
        _mainCamera = GameObject.FindWithTag("MainCamera");
        _cameraShake = _mainCamera.GetComponent<CameraShake>();
        _thrusterPower = _thrusterPowerMax;
        _damageEffect = _shieldObject.GetComponent<ShieldDamageEffect>();
        _normalSpeed = _speed;
        _timer = FindObjectOfType<Timer>();



       // StartControls();
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Translate(Vector3.right * 5 * Time.deltaTime);
        if (_canMove)
        {
            PlayerMovement();
            PlayerButtons();
            ThrusterControl();
        }
        //do the Math.Clamp
    }

    public void StartControls()
    {
        _canMove = true;
    }

    public void PlayerMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0f);
        transform.Translate(direction * _speed * Time.deltaTime);

        if(transform.position.y >= _topBorder)
        {
            transform.position = new Vector3(transform.position.x, _topBorder, 0f);   //1.06
        }
        else if (transform.position.y < _bottomBorder)
        {
            transform.position = new Vector3(transform.position.x, _bottomBorder, 0f);
        }

        if (transform.position.x > _sideBorders)
        {
            transform.position = new Vector3(-_sideBorders, transform.position.y, 0f);
        }
        else if (transform.position.x < -_sideBorders)
        {
            transform.position = new Vector3(_sideBorders, transform.position.y, 0f);
        }   
    }

    public int getLives()
    {
        return _lives;
    }

    public void PlayerButtons()
    {
        if (Input.GetKeyDown(KeyCode.Return) && Time.time > _canFire && (_ammo > 0 || _isImmortal))
        {
            _canFire = Time.time + _fireRate;
            Instantiate(_laserPrefab, new Vector3(transform.position.x, transform.position.y + _fireOffset, 0f), Quaternion.identity);
            if (_isImmortal)
            {
               Instantiate(_laserPrefab, new Vector3(transform.position.x, transform.position.y + _fireOffset, 0f), Quaternion.Euler(0f, 0f, _immortalLaserAngle));
               Instantiate(_laserPrefab, new Vector3(transform.position.x, transform.position.y + _fireOffset, 0f), Quaternion.Euler(0f, 0f, -(_immortalLaserAngle)));
            }
            if (_isTripleShotActive)
            {
                Instantiate(_laserPrefab, new Vector3(transform.position.x - _tripleLaserXOffSet, transform.position.y - _tripleLaserYOffSet, 0f), Quaternion.identity);
                Instantiate(_laserPrefab, new Vector3(transform.position.x + _tripleLaserXOffSet, transform.position.y - _tripleLaserYOffSet, 0f), Quaternion.identity);
            }
            if (!(_isImmortal) && _spawnManagerScript.GetStartState())
            {
                AmmoChanged(-1);
            }
        }

        if (Input.GetKey(KeyCode.RightShift))
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) {
                if (_canUseThruster)
                {
                    _speed = _thrusterSpeed;
                    if (_spawnManagerScript.GetStartState())
                    {
                        _thrusterPower -= _thrusterDrainedPerFrame;
                        _uiManager.ThrusterBarUpdated(_thrusterPower);
                        if (_thrusterPower <= 0f)
                        {
                            _canUseThruster = false;
                            _uiManager.ThrusterText("Refilling...");
                            _speed = _normalSpeed;
                        }
                    }
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.RightShift))
        {
            _speed = _normalSpeed;
        }

        if (Input.GetKeyDown(KeyCode.Keypad4) && _powerUpCaller > 0 && (int)(_spawnManagerScript.GetCurrentRound()) >= _roundItStartsCalling)
        {
            Powerup[] powerUps = FindObjectsOfType<Powerup>();
            if (powerUps.Length > 0)
            {
                foreach (Powerup powerUp in powerUps)
                {
                    powerUp.Called();
                }
                _powerUpCaller--;
            }
        }
    }

    public void ThrusterControl()
    {
        if (_thrusterPower < _thrusterPowerMax && _canUseThruster == false)
        {
            _thrusterPower += _thrusterRefillPerFrame; 
            _uiManager.ThrusterBarUpdated(_thrusterPower);
        }
        else if (_thrusterPower >= _thrusterPowerMax)
        {
            _canUseThruster = true;
            _uiManager.ThrusterText("Thruster Fuel");
        }
    }

    public void Damage(int amount)
    {
        
        _cameraShake.StartShake(0.2f, 0.05f);
        if (_shieldStrength <= 0)
        {
            if (!(_isImmortal))
            {
                while (_lives > 0 && amount > 0)
                {
                    _lives--;
                    amount--;
                    _uiManager.LivesUpdated(_lives, true);
                    switch (_lives)
                    {
                        case 0:
                            Die();
                            break;
                        case 1:
                            _rightFireBall.SetActive(true);
                            _leftFireBall.SetActive(true);
                            break;
                        case 2:
                            _leftFireBall.SetActive(true);
                            _rightFireBall.SetActive(false);
                            break;
                    }
                }
            }
        }
        else
        {
            _shieldStrength--;
            switch (_shieldStrength)
            {
                case 0:
                    _shieldObject.SetActive(false);
                    break;
                default:
                    _damageEffect.FlickerOnHit();
                    break;
            }
        }
    }

    public void ReviveCollected()
    {
        if (_lives < 5)
        {
            _lives++;
            _uiManager.LivesUpdated(_lives, false);
        }
        switch (_lives)
        {
            case 2:
                _leftFireBall.SetActive(true);
                _rightFireBall.SetActive(false);
                break;
            case 3:
                _leftFireBall.SetActive(false);
                _rightFireBall.SetActive(false);
                break;
        }
    }

    public void AddPoints(int numOfPoints)
    {
        _points = _points + numOfPoints;
    }

    //POWER-UPS
    public void CallTripleShot()
    {
        StopCoroutine(TripleShotCoolDown());
        _isTripleShotActive = true;
        StartCoroutine(TripleShotCoolDown());
    }

    IEnumerator TripleShotCoolDown()
    {
        yield return new WaitForSeconds(_tripleShot);
        _isTripleShotActive = false;
    }

    public void CallSpeedBoost()
    {
        _speed = _speed + _powerUpSpeedBoost;
        StartCoroutine(SpeedBoostCoolDown());
    }

    IEnumerator SpeedBoostCoolDown()
    {
        yield return new WaitForSeconds(_speedBoost);
        _speed = _speed - _powerUpSpeedBoost;
    }

    public void CallShield()
    {
        _shieldStrength = 3;
        _shieldObject.SetActive(true);
        _damageEffect.ResetShield();
        //Set Shield to true, set shield strength to three, and set brightness to %100
    }

    public void AmmoChanged(int ammoAdded) // ammoAdded is the amount of Ammo we want to change by whenever this method is called. If the number is negative, we will lose Ammo. If it is positive, we will gain.
    {
        _ammo += ammoAdded;  // Adds the change to our current amount of Ammo, whether that change be positive or negative.

        //However...
        if(_ammo < 0) // If our current amount of Ammo ever attempts to go below zero:
        {
            _ammo = 0; // We set it back up to zero, ensuring this will always be our minimum.
        }
        else if(_ammo > _maxAmmo) // On the other hand, if our current Ammo ever attempts to go above our Maximum Limit:
        {
            _ammo = _maxAmmo; // We set it back down to equal our Maximum, ensuring this is the highest we can go before stopping.
        }
        _uiManager.AmmoUpdated(_ammo, _maxAmmo); // We input _ammo and _maxAmmo as the numbers that should fill in the text.
    }


    public void ImmortalCollected()
    {
        StopCoroutine(ImmortalRoutine());
        _isImmortal = true;
        StartCoroutine(ImmortalRoutine());
    }

    IEnumerator ImmortalRoutine()
    {
        for(int i = 0; i < 5; i++)
        {
            _spriteRenderer.color = Color.yellow;
            yield return new WaitForSeconds(0.07f);
            _spriteRenderer.color = _originalColor;
            yield return new WaitForSeconds(0.07f);
        }
        _spriteRenderer.color = Color.yellow;
        yield return new WaitForSeconds(_immortality);
        _spriteRenderer.color = _originalColor;
        _isImmortal = false;
    }

    public void HeatSeekingMissileCollected()
    {
        StopCoroutine(MissileCoolDown());
        _laserPrefab = _missilePrefab;
        StartCoroutine(MissileCoolDown());
    }

    IEnumerator MissileCoolDown()
    {
        yield return new WaitForSeconds(_heatSeekingMissile);
        _laserPrefab = _normalLaserPrefab;
    }

    public void ResetCaller()
    {
        _powerUpCaller = _numOfCallsPerRound;
    }

    public void Die()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        if (enemies.Length > 0)
        {
            foreach (Enemy enemy in enemies)
            {
                enemy.OnPlayerDeath();
            }
        }
        Boss boss = FindObjectOfType<Boss>();
        if (boss != null)
        {
            boss.OnPlayerDeath();
        }
        _canMove = false;
        _spawnManagerScript.OnPlayerDeath();
        _timer.StopTimer();
        if (_gameOverTimeline != null)
        {
            _gameOverTimeline.Play();
        }
        Instantiate(_explosion, transform.position, Quaternion.identity);
        Destroy(this.gameObject, 0.25f);
    }
}