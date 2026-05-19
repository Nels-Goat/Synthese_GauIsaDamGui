using UnityEngine;

public class PUStaff : Powerup
{


    public PUStaff(Sprite icon)
    {
        _icon = icon;
        _name = EPowerupType.Apocalypse;
        _description = "Amélioration de la masse";
        _level = 0;
        _hidden = false;
    }

    public override void Upgrade()
    {
        if (_hidden) return;
        _level++;


        // === Effet === //

        Transform player = Object.FindFirstObjectByType<Player>().transform;
        PlayerStaff staff = player.GetChild(0).gameObject.GetComponent<PlayerStaff>();
        staff.SetWeaponLevel(_level);


        foreach (Transform t in player.transform)
            t.gameObject.SetActive(false);

        
        staff.gameObject.SetActive(true);

        // ============= //


        if (_level >= 3) _hidden = true;
    }
}