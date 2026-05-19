using UnityEngine;

public class Heal : Powerup
{
    
    public Heal(Sprite icon)
    {
        _icon = icon;
        _name = EPowerupType.Rafraîchissement;
        _description = "Soigne 10% des PVs totaux";
        _level = 0;
        _hidden = true;
    }

    public override void Upgrade()
    {
        Player player = Object.FindFirstObjectByType<Player>();
        float heal = player.PlayerMaxLife * .1f;
        float newLife = heal + player.PlayerLife;

        if (newLife > player.PlayerMaxLife)
            player.PlayerLife = player.PlayerMaxLife;
        else
            player.PlayerLife += heal;
    }
}