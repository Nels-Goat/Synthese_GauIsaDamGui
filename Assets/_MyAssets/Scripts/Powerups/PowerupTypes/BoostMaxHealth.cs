using UnityEngine;

public class BoostMaxHealth : Powerup
{
    public BoostMaxHealth(Sprite icon)
    {
        _icon = icon;
        _name = EPowerupType.Renforcement;
        _description = "Augmente le max de PVs";
        _level = 0;
    }

    public override void Upgrade()
    {
        if (_hidden) return;

        _level++;

        Player player = Object.FindFirstObjectByType<Player>();
        float currentMax = player.PlayerMaxLife;
        float newMax = currentMax + 5;

        player.PlayerMaxLife = newMax;


        if (_level >= 3) _hidden = true;
    }
}