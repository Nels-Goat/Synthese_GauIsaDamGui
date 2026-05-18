using System.Collections.Generic;
using UnityEngine;

public class PowerupManager : MonoBehaviour
{
    public static PowerupManager Instance;

    [SerializeField] private Sprite[] _listIcon;

    private List<Powerup> _listPowerup;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }


    private void Start()
    {
        _listPowerup = new List<Powerup>
        {
            new GainHealth(_listIcon[0]),
            new BoostMaxHealth(_listIcon[1]),
        };
    }


    public Powerup[] GetPowerups()
    {
        Powerup[] powerups = {_listPowerup[0]};
        return powerups;
    }

    public void UpgradePowerup(EPowerupType name)
    {
        if (name == EPowerupType.none) return;

        _listPowerup[(int)name].Upgrade();
    }
}
