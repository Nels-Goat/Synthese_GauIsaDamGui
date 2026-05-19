using UnityEngine;

public class BetterDashInterval : Powerup
{
    private float _modifier = .5f;

    public BetterDashInterval(Sprite icon)
    {
        _icon = icon;
        _name = EPowerupType.Gymnaste;
        _description = "Réduit l'interval du Dash";
        _level = 0;
        _hidden = false;
    }

    public override void Upgrade()
    {
        if (_hidden) return;
        _level++;


        // === Effet === //

        Player player = Object.FindFirstObjectByType<Player>();

        if (player.PlayerDashRate - _modifier <= 0f)
            return;
        else
            player.PlayerDashRate -= _modifier;

        // ============= //


        if (_level >= 3) _hidden = true;
    }
}