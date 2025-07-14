using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleScript : MonoBehaviour
{

    private int _specifiedNumber;
    // Creating and naming the new variable type
    private enum Direction 
    {
        // The names of the values you'd like the variable to have
        Up,                
        Down,
        Left,
        Right,
        WhateverOtherDirectionsYouWant,
    }

    // An instantiation of said new variable
    private Direction _currentDirection = Direction.Left;

    // An example on how to change the variable
    void Start()
    {
        _currentDirection = Direction.Up;




        // Instantiating an Array and Designating its size.
        int[] _arrayExample = new int[8];

        // Setting the slots
        _arrayExample[0] = 34;
        _arrayExample[1] = 58;
        _arrayExample[2] = 98;
        // ...and so on



        // Instantiating an Array and Designating its size.
        List<int> _listExample = new List<int>();

       // Filling it with as many variables as you want
        _listExample.Add(34);
        _listExample.Add(58);
        _listExample.Add(98);









    }

    // ...And finally, putting the variable to work
    void Update()
    {
        
        if (_currentDirection == Direction.Left)
        {
            // move the object left
        }
    }
}

