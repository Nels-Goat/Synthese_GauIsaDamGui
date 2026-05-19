using UnityEngine;

public abstract class Powerup
{
    protected Sprite _icon;
    protected EPowerupType _name;
    protected string _description;
    protected int _level;

    public EPowerupType Name {get => _name;}
    public string Description {get => _description;}
    public Sprite Icon {get => _icon;}
    public int Level {get => _level;}

    public abstract void Upgrade();
}