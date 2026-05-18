using System.Collections.Generic;
using UnityEngine;

public class PowerupManager : MonoBehaviour
{
    public PowerupManager Instance;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
