using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Boss : MonoBehaviour
{
    [SerializeField]
    private AudioSource BossMusic;


    [SerializeField]
    private Turret _extLeftTurret, _extRightTurret;
    [SerializeField]
    private Turret[] _intLeftTurret, _intRightTurret;


    [SerializeField]
    private float _minFirePause, _maxFirePause;
    [SerializeField]
    private float _minXPosition, _maxXPosition;
    [SerializeField]
    private int _minCasualFires, _maxCasualFires;



    [SerializeField]
    private bool _isMoving;
    [SerializeField]
    private float _movingSpeed;
    [SerializeField]
    private float _currentXPosition;
    [SerializeField]
    private int _health;
    [SerializeField]
    public bool _halfDead;


    private int _currentHealth;
    [SerializeField]
    private bool _canBeHurt;
    [SerializeField]
    private SpriteRenderer _spriteRenderer;
    [SerializeField]
    private Color _originalColor;
    [SerializeField]
    private List<Color> _colorList;
    [SerializeField]
    private SpawnManager _spawnManager;
    [SerializeField]
    private Turret[] _littleExplosionSpawners;
    [SerializeField]
    private Turret _bigExplosion;
    [SerializeField]
    private UIManager _uiManager;
    [SerializeField]
    private Timer _timer;
    [SerializeField]
    private int _numOfIntShotsPerRound;
    [SerializeField]
    private bool _isPlayerAlive = true;
    
    

    // Start is called before the first frame update
    void Start()
    {
        _currentHealth = _health;
        _spawnManager = FindObjectOfType<SpawnManager>();
        _uiManager = FindObjectOfType<UIManager>();
        _timer = FindObjectOfType<Timer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isMoving)
        {
            Vector3 wantedPosition = new Vector3(_currentXPosition, transform.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, wantedPosition, _movingSpeed * Time.deltaTime);
            if (transform.position == wantedPosition)
            {
                _currentXPosition = Random.Range(_minXPosition, _maxXPosition);
            }
        }
    }

    public void StartFight()
    {
        _canBeHurt = true;
        StartCoroutine(OutsideFireRoutine());
        _isMoving = true;
    }

    public bool CanBeHurt()
    {
        return _canBeHurt;
    }

    // A Coroutine that fires two large lasers from the outside turrets
    IEnumerator OutsideFireRoutine()
    {
        while (_isPlayerAlive)
        {
            float pause = Random.Range(_minFirePause, _maxFirePause);
            yield return new WaitForSeconds(pause);
            if (_isPlayerAlive)
            {
                _extLeftTurret.Fire();
                _extRightTurret.Fire();
            }
        }
    }

    // Similar to the OutsideFireRoutine, except it fires several tiny lasers from the inside turrets all at once, not to mention fires three times in rapid succession
    IEnumerator InsideFireRoutine()
    {
        while (_isPlayerAlive)
        {
            float pause = Random.Range(_minFirePause, _maxFirePause);
            yield return new WaitForSeconds(pause);
            for(int i = 0; i < _numOfIntShotsPerRound; i++)
            {
                if (_isPlayerAlive)
                {
                    foreach (var turret in _intLeftTurret)
                    {
                        turret.Fire();
                    }
                    foreach (var turret in _intRightTurret)
                    {
                        turret.Fire();
                    }
                    yield return new WaitForSeconds(0.2f);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Laser" && _canBeHurt)
        {
            Laser laser = other.GetComponent<Laser>();
            if (laser.IsEnemyLaser() == false)
            {
                Destroy(other.gameObject);
                _currentHealth--;
                if(_currentHealth > 0)
                {
                    for (int i = _colorList.Count - 1; i > 0; i--)
                    {
                        if (_currentHealth <= (float)( ((float)(_colorList.Count - i) / (_colorList.Count)) * _health))
                        {
                            _originalColor = _colorList[i];
                            break;
                        }
                    }
                    if(_currentHealth <= (float)(_health/2.0) && _halfDead == false)
                    {
                        _halfDead = true;
                        StartCoroutine(InsideFireRoutine());
                    }
                    StopCoroutine(Damage());
                    StartCoroutine(Damage());
                }
                else 
                {
                    _originalColor = _colorList[0];
                    _spriteRenderer.color = _originalColor;
                    _canBeHurt = false;
                    StopAllCoroutines();
                    StartCoroutine(Danger());
                    StartCoroutine(Die());
                }
            }
        }
    }

    public void OnPlayerDeath()
    {
        _isPlayerAlive = false;
        StopCoroutine(OutsideFireRoutine());
        StopCoroutine(InsideFireRoutine());
        StopCoroutine(Die());
    }

    IEnumerator Damage()
    {
        _spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        _spriteRenderer.color = _originalColor;
    }

    IEnumerator Danger()
    {
        while (true)
        {
            _spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.04f);
            _spriteRenderer.color = _originalColor;
            yield return new WaitForSeconds(0.04f);
        }
    }

    IEnumerator Die()
    {
        _uiManager.OnGameCompleted();
        _timer.StopTimer();
        _spawnManager.OnBossDefeated();
        _movingSpeed = 0;
        foreach(var explosion in _littleExplosionSpawners)
        {
            explosion.Fire();
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(1.45f);
        _bigExplosion.Fire();
        _uiManager.OnPlayerWin();
        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);
    }
}