using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField]
    private UIManager _uiManager;
    [SerializeField]
    private bool _isRoundGoing;
    // Start is called before the first frame update
    void Start()
    {
        _uiManager = FindObjectOfType<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartTimer()
    {
        _isRoundGoing = true;
        StartCoroutine(TimerRoutine());
    }

    public void StopTimer()
    {
        _isRoundGoing = false;
    }

    IEnumerator TimerRoutine()
    {
        int _seconds = 0;
        int _minutes = 0;
        //Count the time before the player dies.
        yield return new WaitForSeconds(1.0f);
        while (_isRoundGoing == true)
        {
            //plus _seconds variable by one
            _seconds++;

            //if seconds is greater than or equal to 60, minus 60 from it and add one to the minutes
            if (_seconds >= 60)
            {
                _seconds = _seconds - 60;
                _minutes++;
            }

            //wait one second
            yield return new WaitForSeconds(1.0f);
        }

        _uiManager.TimerUpdated(_seconds, _minutes);
    }
}