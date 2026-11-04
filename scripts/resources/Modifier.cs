using Godot;

namespace Game.Resources;

[GlobalClass]
public partial class Modifier(Stat Stat, Modifier.OperationType Operation, float Value) : Resource
{
    public enum OperationType
    {
        Add,
        Subtract,
        Multiply,
        Divide
    }

    [Export]
    public Stat Stat = Stat;

    [Export]
    public OperationType Operation = Operation;

    [Export]
    public float Value = Value;

    public Modifier() : this(null, OperationType.Add, 0) {}
}
