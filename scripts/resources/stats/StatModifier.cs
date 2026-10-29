using Godot;

namespace Game.Resources.Stats;

public partial class StatModifier : Resource
{
    public enum OperationType
    {
        Add,
        Subtract,
        Multiply,
        Divide
    }

    [Export]
    public string AttributeName;

    [Export]
    public OperationType Operation;

    [Export]
    public float Value;
}
