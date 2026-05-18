using UnityEngine;

public class GainHealth : Powerup
{
    public GainHealth()
    {
        _name = "Regénération";
        _description = "Soigne 50% de tes PVs";
        _icon = null;
        _level = 0;
    }

    public override void Upgrade()
    {
        Player player = GameObject.FindFirstObjectByType<Player>();
        float halfLife = player.PlayerMaxLife * .5f;
        float toHeal = player.PlayerMaxLife - player.PlayerLife;

        if (toHeal > player.PlayerMaxLife)
            player.PlayerLife = player.PlayerMaxLife;
        else
            player.PlayerLife += halfLife;

    }
}