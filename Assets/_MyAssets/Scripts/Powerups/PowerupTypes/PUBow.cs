using UnityEngine;

public class PUBow : Powerup
{


    public PUBow(Sprite icon)
    {
        _icon = icon;
        _name = EPowerupType.Arc;
        _description = "Amélioration de l'arc";
        _level = 1;
        _hidden = false;
    }

    public override void Upgrade()
    {
        if (_hidden) return;
        _level++;


        // === Effet === //

        Transform player = Object.FindFirstObjectByType<Player>().transform;
        PlayerBow bow = player.GetChild(1).gameObject.GetComponent<PlayerBow>();
        bow.SetWeaponLevel(_level);

        foreach (Transform t in player)
            t.gameObject.SetActive(false);

        
        bow.gameObject.SetActive(true);

        // ============= //


        if (_level >= 3) _hidden = true;
    }
}