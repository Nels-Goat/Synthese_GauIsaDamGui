using UnityEngine;

public class PUSword : Powerup
{


    public PUSword(Sprite icon)
    {
        _icon = icon;
        _name = EPowerupType.Épée;
        _description = "Amélioration de l'épée";
        _level = 0;
        _hidden = false;
    }

    public override void Upgrade()
    {
        if (_hidden) return;
        _level++;


        // === Effet === //

        Transform player = Object.FindFirstObjectByType<Player>().transform;
        PlayerSword sword = player.GetChild(2).gameObject.GetComponent<PlayerSword>();
        sword.SetWeaponLevel(_level);

        foreach (Transform t in player.transform)
            t.gameObject.SetActive(false);

        sword.gameObject.SetActive(true);

        // ============= //


        if (_level >= 3) _hidden = true;
    }
}