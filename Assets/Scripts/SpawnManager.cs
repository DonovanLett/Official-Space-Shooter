using System.Collections;
using System.Collections.Generic;
//using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class PowerupsAddedEachRound
{
    public List<GameObject> powerUps;
}

[System.Serializable]
public class EnemiesAddedEachRound
{
    public List<GameObject> enemies;
}

public class SpawnManager : MonoBehaviour
{
     public enum Round{
        FirstRound,
        SecondRound,
        ThirdRound,
        FourthRound,
        FifthRound,
        SixthRound,
        SeventhRound,
        FinalRound,
     }
    
    private Round _currentRound = Round.FirstRound;

    [SerializeField]
    private List<GameObject> _enemyPrefab;
    [SerializeField]
    private List <GameObject> _powerUpPrefab;
    [SerializeField]
    private GameObject _powerUpContainer;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private bool _isStarted;
    [SerializeField]
    private bool _isSpawning = false;
    [SerializeField]
    private UIManager _uiManager;
    [SerializeField]
    private bool _isPlayerDead;
    [SerializeField]
    private List<PowerupsAddedEachRound> powerUpsAddedEachRound;
    [SerializeField]
    private List<EnemiesAddedEachRound> enemiesAddedEachRound;
    [SerializeField]
    private float _lengthOfEachRound;
    [SerializeField]
    private Player _player;
    [SerializeField]
    private AudioSource _roundMusic;
    [SerializeField]
    private PlayableDirector _bossTimeline;
    [SerializeField]
    private AudioSource _bossMusic;
    [SerializeField]
    private float _minEnemySpawnPause, _maxEnemySpawnPause; /// 0.5, 1.5            
    [SerializeField]
    private float _minPowerupSpawnPause, _maxPowerupSpawnPause;  /// 3, 7


    public Round GetCurrentRound()
    {
        return _currentRound;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _uiManager = FindObjectOfType<UIManager>();
        _isPlayerDead = false;
        _player = FindObjectOfType<Player>();
    }

    public void OnRoundStart()
    {
        if(_isStarted == false)
        {
            _isStarted = true;
            _roundMusic.Play();
        }
        _isSpawning = true;
        StartCoroutine(EnemyExperimentalSpawnRoutine());
        StartCoroutine(PowerUpExperimentalSpawnRoutine());
        StartCoroutine(CycleThroughRounds());
    }

    public bool GetStartState()
    {
        return _isStarted;
    }

    IEnumerator EnemyExperimentalSpawnRoutine()
    {
        yield return new WaitForSeconds(Random.Range(_minEnemySpawnPause, _maxEnemySpawnPause));
        while (_isSpawning == true)
        {
            int sumOfChances = 0;
            for (int i = 0; i < _enemyPrefab.Count; i++)
            {
                if (_enemyPrefab[i].GetComponent<Enemy>().GetChance() > 0)
                {
                    sumOfChances += _enemyPrefab[i].GetComponent<Enemy>().GetChance();
                }
            }
            int randomChance = (Random.Range(1, sumOfChances + 1));
            for (int i = 0; i < _enemyPrefab.Count; i++)
            {
                if (_enemyPrefab[i].GetComponent<Enemy>().GetChance() >= randomChance && _enemyPrefab[i].GetComponent<Enemy>().GetChance() > 0)
                {
                    //float randomX = Random.Range(-9.4f, 9.4f); 7.4 for y
                    GameObject newEnemy = Instantiate(_enemyPrefab[i], _enemyPrefab[i].GetComponent<Enemy>().GetStart(), Quaternion.identity);
                    newEnemy.transform.parent = _enemyContainer.transform;
                    break;
                }
                else
                {
                    if (_enemyPrefab[i].GetComponent<Enemy>().GetChance() > 0)
                    {
                        randomChance -= _enemyPrefab[i].GetComponent<Enemy>().GetChance();
                    }
                }
            }
            yield return new WaitForSeconds(Random.Range(_minEnemySpawnPause, _maxEnemySpawnPause));
        }
    }

    IEnumerator PowerUpExperimentalSpawnRoutine()
    {
        yield return new WaitForSeconds(Random.Range(_minPowerupSpawnPause, _maxPowerupSpawnPause));
        while (_isSpawning == true) 
        {
            int sumOfChances = 0;
            for(int i = 0; i < _powerUpPrefab.Count; i++){
                if (_powerUpPrefab[i].GetComponent<Powerup>().GetChance() > 0)
                {
                    sumOfChances += _powerUpPrefab[i].GetComponent<Powerup>().GetChance();
                }
            }
            int randomChance = (Random.Range(1, sumOfChances + 1));
            for(int i = 0; i < _powerUpPrefab.Count; i++){
                  if(_powerUpPrefab[i].GetComponent<Powerup>().GetChance() >= randomChance && _powerUpPrefab[i].GetComponent<Powerup>().GetChance() > 0){
                     float randomX = Random.Range(-9.4f, 9.4f);
                     GameObject newPowerUp = Instantiate(_powerUpPrefab[i], new Vector3(randomX, 7.4f, 0f), Quaternion.identity);
                     newPowerUp.transform.parent = _powerUpContainer.transform;
                     break;
                  }
                  else{
                       if(_powerUpPrefab[i].GetComponent<Powerup>().GetChance() > 0){
                        randomChance -= _powerUpPrefab[i].GetComponent<Powerup>().GetChance();
                       }
                  }
            }
            yield return new WaitForSeconds(Random.Range(_minPowerupSpawnPause, _maxPowerupSpawnPause));
        }
    }

    IEnumerator CycleThroughRounds()
    {
        if ((_currentRound) != (Round.FinalRound)) {
            yield return new WaitForSeconds(_lengthOfEachRound); //20
            _isSpawning = false;
            while(_enemyContainer.transform.childCount > 0)
            {
                yield return new WaitForEndOfFrame();
            }
            _currentRound = (Round)((int)(_currentRound + 1));
            _uiManager.RoundUpdated((int)(_currentRound));
            if ((_currentRound) != (Round.FinalRound))
            {
                for (int i = 0; i < powerUpsAddedEachRound[(int)(_currentRound)].powerUps.Count; i++)
                {
                    _powerUpPrefab.Add(powerUpsAddedEachRound[(int)(_currentRound)].powerUps[i]);
                }
                for (int i = 0; i < enemiesAddedEachRound[(int)(_currentRound)].enemies.Count; i++)
                {
                    _enemyPrefab.Add(enemiesAddedEachRound[(int)(_currentRound)].enemies[i]);
                }
                yield return new WaitForSeconds(1.5f);
                if (!(_isPlayerDead))
                {
                    _player.ResetCaller();
                    _uiManager.StartWaveText((int)(_currentRound));
                }
            }
            else
            {
                yield return new WaitForSeconds(1.5f);
                if (!(_isPlayerDead))
                {
                    _roundMusic.Stop();
                    _bossTimeline.Play();
                }

            }
        }
    }


    public void StartBossMusic()
    {
        _bossMusic.Play();
        _isSpawning = true;
        StartCoroutine(PowerUpExperimentalSpawnRoutine());
    }

    public void OnPlayerDeath()
    {
        _isSpawning = false;
        _isPlayerDead = true;
        _roundMusic.Stop();
        _bossMusic.Stop();
    }

    public void OnBossDefeated()
    {
        _bossMusic.Stop();
        _isSpawning = false;
        StopCoroutine(PowerUpExperimentalSpawnRoutine());
    }

    
}