using UnityEngine;

public class GainHealth : Powerup
{
    
    public GainHealth(Sprite icon)
    {
        _icon = icon;
        _name = EPowerupType.Regénération;
        _description = "Soigne 50% des PVs totaux";
        _level = 0;
    }

    public override void Upgrade()
    {
        if (_hidden) return;

        _level++;

        Player player = Object.FindFirstObjectByType<Player>();
        float halfLife = player.PlayerMaxLife * .5f;
        float newLife = halfLife + player.PlayerLife;

        if (newLife > player.PlayerMaxLife)
            player.PlayerLife = player.PlayerMaxLife;
        else
            player.PlayerLife += halfLife;


        if (_level >= 3) _hidden = true;
    }
}