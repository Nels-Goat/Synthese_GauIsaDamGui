using UnityEngine;

public abstract class Powerup
{
    protected Sprite _icon = null;
    protected EPowerupType _name = EPowerupType.None;
    protected string _description = "Nouveau PU";
    protected int _level = 0;
    protected bool _hidden = false;

    public EPowerupType Name {get => _name;}
    public string Description {get => _description;}
    public Sprite Icon {get => _icon;}
    public int Level {get => _level;}
    public bool Hidden {get => _hidden;}

    public abstract void Upgrade();
}