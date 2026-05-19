using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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


    public List<Powerup> GetPowerups()
    {
        List<Powerup> available = new List<Powerup>();
        List<Powerup> powerups = new List<Powerup>();

        foreach (Powerup pu in _listPowerup) {
            if (pu.Level < 3) available.Add(pu);
            Debug.Log(available);
        }
        available = reshuffle(available);

        int len = available.Count > 3 ? 3 : available.Count;
        for (int i = 0; i < len; i++)
            powerups.Add(available[i]);

        return powerups;
    }

    public void UpgradePowerup(EPowerupType name)
    {
        if (name == EPowerupType.none) return;

        _listPowerup[(int)name].Upgrade();
    }

    private List<Powerup> reshuffle(List<Powerup> pu)
    {
        for (int t = 0; t < pu.Count; t++)
        {
            Powerup tmp = pu[t];
            int r = Random.Range(t, pu.Count);
            pu[t] = pu[r];
            pu[r] = tmp;
        }
        return pu;
    }

}
