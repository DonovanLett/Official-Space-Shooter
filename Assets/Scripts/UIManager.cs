//using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
//using UnityEngine.UIElements;


[System.Serializable]
public class SmallSentences
{
    public List<string> _sentencesInEachRoundText; // Sentences for current round
}



public class UIManager : MonoBehaviour
{
    [SerializeField]
    private PostProcessProfile _mainPostProcessor;
    [SerializeField]
    private SpawnManager _spawnManager;
    [SerializeField]
    private Text _time;
    [SerializeField]
    private Text _powerUpsTally;
    [SerializeField]
    private Text _enemiesKilledTally;
    [SerializeField]
    private int _enemiesKilled = 0;
    [SerializeField]
    private Text _pointTally;
    [SerializeField]
    private int _powerUpsCollected = 0;
    [SerializeField]
    private Text _waveText;
    [SerializeField]
    private int _points = 0;
    [SerializeField]
    private GameObject _onScreenInfo;
    [SerializeField]
    private Image[] _hearts;
    [SerializeField]
    private Text _thrusterText;
    [SerializeField]
    private Text _smallPointText;
    [SerializeField]
    private TMP_Text _ammoText;
    [SerializeField]
    private Image _thrusterBar;
    [SerializeField]
    private Text _thrusterPercentText;
    [SerializeField]
    private GameObject _camera;
    [SerializeField]
    private PlayableDirector _fadeInTimeline;
    [SerializeField]
    private FirstTimeAttempt _firstTimeAttempt;
    [SerializeField]
    private PlayableDirector _winnerTimeline;



    [Header("Wave Text")]
    [SerializeField]
    private Text _bigWaveText;
    [SerializeField]
    private string[] _bigWaveTextSentences;
    [SerializeField]
    private Text _smallerWaveText;
    [SerializeField]
    private List<SmallSentences> _smallerWaveTextSentences;







    [SerializeField]
    private AudioClip _typeWriterSoundEffect;
    [SerializeField]
    private float _delayBetweenEachLetter;
    [SerializeField]
    private float _delayBetweenEachSentence;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = FindObjectOfType<SpawnManager>();
        _camera = GameObject.FindWithTag("MainCamera");

        if (_firstTimeAttempt.IsFirstTime())
        {
            StartWaveText(0);
            _firstTimeAttempt.ChangeFirstTime();
        }
        else
        {
            _fadeInTimeline.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowOnScreenUI()
    {

        _onScreenInfo.SetActive(true);
    }

    public void LivesUpdated(int lives, bool _isBeingDamaged)
    {
        if (_isBeingDamaged)
        {
            _hearts[lives].color = Color.black;
            if (lives == 0)
            {
                _onScreenInfo.SetActive(false);
                _smallPointText.text = "";
            }
        }
        else
        {
            if (lives <= 5)
            {
                _hearts[lives - 1].color = Color.red;
            }
        }
    }

    public void OnPlayerWin()
    {
        _winnerTimeline.Play();
    }

    public void ThrusterBarUpdated(float _thruster)
    {
        _thrusterBar.fillAmount = _thruster;
        int _percent = (int)(_thruster * 100f);  // Multiplies the Thruster Power number we are given by 100 and converts it to an int, effectively changing our decimal value to a proper percentage value
        _thrusterPercentText.text = _percent.ToString() + "%";  //Converts the number to a String, adds a "%" at the end of it, and changes _thrusterPercentText to read it
    }

    public void AmmoUpdated(int currentAmmo, int currentMaxAmmo)
    {
        _ammoText.text = "AMMO: " + currentAmmo + "/" + currentMaxAmmo;
    }

    //Points
    public void PointsAdded(int points)
    {
        _points += points;
        _smallPointText.text = "SCORE - " + _points;
        _pointTally.text = "FINAL SCORE - " + _points;
    }

    //EnemiesKilled
    public void EnemyKilled()
    {
        _enemiesKilled++;
        _enemiesKilledTally.text = "ENEMIES KILLED - " + _enemiesKilled;
    }

    //PowerUps
    public void PowerUpCollected()
    {
        _powerUpsCollected++;
       _powerUpsTally.text = "POWER-UPS COLLECTED - " + _powerUpsCollected;
    }

    // public void Round Update
    public void RoundUpdated(int wave)
    {
        _waveText.text = "ROUNDS COMPLETED - " + wave;
    }

   

    public void TimerUpdated(int _seconds, int _minutes)
    {
       
        //Convert Minutes and Seconds into Text for the Tally
        string _minuteWord = "Minutes";
        string _secondWord = "Seconds";
        if (_seconds == 1)
        {
            _secondWord = "Second";
        }
        if(_minutes == 1)
        {
           _minuteWord = "Minute";
        }
        if (_seconds > 0 && _minutes > 0)
        {
            _time.text = "TIME - " + _minutes + " " + _minuteWord + " and " + _seconds + " " + _secondWord;
        }
        else if (_seconds > 0 && _minutes <= 0)
        {
            _time.text = "TIME - " + _seconds + " " + _secondWord;
        }
        else if(_seconds <= 0 && _minutes > 0)
        {
            _time.text = "TIME - " + _minutes + " " + _minuteWord;
        }
        else
        {
            _time.text = "No Time Recorded. There might be something wrong with the Timer.";
        }
    }

    public void ThrusterText(string _text)
    {
        _thrusterText.text = _text;
    }

    public void StartWaveText(int wave)
    {
        StartCoroutine(GoThroughWaveText(wave));        // Start the Coroutine below to go through the current wave text
    }
    IEnumerator GoThroughWaveText(int wave)
    {
        // THIS GOES THROUGH EACH CHARACTER OF THE SENTENCE WE WANT TO CURRENTLY PRINT, ADDING THEM TO OUR "BIG WAVE TEXT" ONE AT A TIME WITH A SMALL DELAY IN BETWEEN EACH ONE, AND HOLDING ON THE WHOLE SENTENCE FOR A SECOND BEFORE CLEARING IT AND MOVING ON
        if (!(_bigWaveTextSentences[wave].Equals("")))
        {
            foreach (char letter in _bigWaveTextSentences[wave])
            {
                AudioSource.PlayClipAtPoint(_typeWriterSoundEffect, _camera.transform.position, 90f);
                _bigWaveText.text += letter;
                yield return new WaitForSeconds(_delayBetweenEachLetter);
            }
            yield return new WaitForSeconds(_delayBetweenEachSentence);
        }
        _bigWaveText.text = "";



        for (int i = 0; i < _smallerWaveTextSentences[wave]._sentencesInEachRoundText.Count; i++)
        {
            if (!(_smallerWaveTextSentences[wave]._sentencesInEachRoundText[i].Equals("")))
            {
                foreach (char letter in _smallerWaveTextSentences[wave]._sentencesInEachRoundText[i])
                {
                    AudioSource.PlayClipAtPoint(_typeWriterSoundEffect, _camera.transform.position, 90f);
                    _smallerWaveText.text += letter;
                    yield return new WaitForSeconds(_delayBetweenEachLetter);
                }
                yield return new WaitForSeconds(_delayBetweenEachSentence);
            }
            _smallerWaveText.text = "";
        }
        if (wave == 0)
        {
            _fadeInTimeline.Play();
        }
        else
        {
            _spawnManager.OnRoundStart();
        }
    }

    public void OnGameCompleted()
    {
        _onScreenInfo.SetActive(false);
        _waveText.text = "ROUNDS COMPLETED - All 8";
    }
}