using UnityEngine;

public class Invincible : Powerup
{
    private float _modifier = .25f;

    public Invincible(Sprite icon)
    {
        _icon = icon;
        _name = EPowerupType.Invincible;
        _description = "Augment la durée d'invincibilité";
        _level = 0;
        _hidden = false;
    }

    public override void Upgrade()
    {
        if (_hidden) return;
        _level++;


        // === Effet === //

        Player player = Object.FindFirstObjectByType<Player>();

        player.IFramesDuration += _modifier;

        // ============= //


        if (_level >= 3) _hidden = true;
    }
}