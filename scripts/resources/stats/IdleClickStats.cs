using Godot;

namespace Game.Resources.Stats;

[GlobalClass]
public partial class IdleClickStats : BaseStats
{
    [Export]
    public int Radius;

    [Export]
    public int Cooldown;

    [Export]
    public int BaseDamage;

    [Export]
    public float CriticalDamageChance;

    public IdleClickStats() {}
}
