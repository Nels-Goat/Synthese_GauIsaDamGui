using UnityEngine;

public abstract class Powerup
{
    protected string _name;
    protected string _description;
    protected Sprite _icon;
    protected int _level;

    public string Name {get;}
    public string Description {get;}
    public int Level {get;}

    public abstract void Upgrade();
}