using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstTimeAttempt : MonoBehaviour
{
    [SerializeField]
    private static bool firstTime = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public bool IsFirstTime()
    {
        return firstTime;
    }

    public void ChangeFirstTime()
    {
        firstTime = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
