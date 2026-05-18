using System.Collections.Generic;
using UnityEngine;

public class PowerupManager : MonoBehaviour
{
    public static PowerupManager Instance;

    private List<Powerup> _listPowerup;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }


    private void Start()
    {
        _listPowerup.Add(new GainHealth());
    }


    // Update is called once per frame
    private void Update()
    {
        
    }

    public Powerup[] GetPowerups()
    {
        Powerup[] powerups = {_listPowerup[0]};
        return powerups;
    }
}
