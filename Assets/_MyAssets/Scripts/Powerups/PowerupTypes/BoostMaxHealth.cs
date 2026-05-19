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
        if (_level >= 3) return;

        _level++;

        Player player = Object.FindFirstObjectByType<Player>();
        float currentMax = player.PlayerMaxLife;
        float newMax = currentMax * (1 +  _level * .5f);

        player.PlayerMaxLife = newMax;
    }
}