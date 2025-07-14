using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{

    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private bool _isShootingDown;
    [SerializeField]
    private bool _canDestroyPowerUp;
    // Start is called before the first frame update
    void Start()
    {
        
    }



    // Update is called once per frame
    void Update()
    {
        
    }


    public void Fire()
    {
        GameObject laser = Instantiate(_laserPrefab, transform.position, transform.rotation);
        if (laser.GetComponent<Laser>() != null)
        {
            laser.GetComponent<Laser>().SetVariables(true, _isShootingDown, _canDestroyPowerUp);
        }
    }
}
