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
            new Heal(_listIcon[(int)EPowerupType.Rafraîchissement]),
            new GainHealth(_listIcon[(int)EPowerupType.Regénération]),
            new BoostMaxHealth(_listIcon[(int)EPowerupType.Renforcement]),
            new BetterDashInterval(_listIcon[(int)EPowerupType.Gymnaste]),
            new Invincible(_listIcon[(int)EPowerupType.Invincible]),
            new PUBow(_listIcon[(int)EPowerupType.Arc]),
            new PUSword(_listIcon[(int)EPowerupType.Épée]),
            new PUStaff(_listIcon[(int)EPowerupType.Apocalypse]),
        };
    }


    public List<Powerup> GetPowerups()
    {
        List<Powerup> available = new List<Powerup>();
        List<Powerup> powerups = new List<Powerup>();

        foreach (Powerup pu in _listPowerup) {
            if (!pu.Hidden) available.Add(pu);
            Debug.Log(available);
        }

        // Si aucune carte disponible, retourne soins
        if (available.Count <= 0)
        {
            for (int i = 0; i < 3; i++)
                available.Add(_listPowerup[(int)EPowerupType.Rafraîchissement]);
            return available;
        }

        available = reshuffle(available);

        int len = available.Count > 3 ? 3 : available.Count;
        for (int i = 0; i < len; i++)
            powerups.Add(available[i]);

        return powerups;
    }

    public void UpgradePowerup(EPowerupType name)
    {
        if (name == EPowerupType.None) return;

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
