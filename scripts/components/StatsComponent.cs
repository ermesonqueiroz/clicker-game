using Game.Resources.Stats;
using Godot;

namespace Game.Components;

[GlobalClass]
public partial class StatsComponent : Node
{
    [Export]
    private BaseStats _baseStats;

    public void ApplyModifier(StatModifier statModifier)
    {
        if (_baseStats.GetType().GetProperty(statModifier.AttributeName) == null)
        {
            GD.PushError($"Attribute not found: '{statModifier.AttributeName}'");
            return;
        }

        float value;
        switch (statModifier.Operation)
        {
            case StatModifier.OperationType.Add:
                value = (float)_baseStats.Get(statModifier.AttributeName) + statModifier.Value;
                break;
            default:
                value = (float)_baseStats.Get(statModifier.AttributeName);
                break;
        }

        _baseStats.Set(statModifier.AttributeName, value);
    }
}
