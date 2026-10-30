using Godot;

namespace Game.Resources.Stats;

[GlobalClass]
public partial class Modifier : Resource
{
    public enum OperationType
    {
        Add,
        Subtract,
        Multiply,
        Divide
    }

    [Export]
    public string StatID = "";

    [Export]
    public OperationType Operation;

    [Export]
    public double Value;

    public Modifier()
    {
    }

    public Modifier(OperationType operation, double value)
    {
        Operation = operation;
        Value = value;
    }

    public Modifier(string statID, OperationType operation, double value)
    {
        StatID = statID;
        Operation = operation;
        Value = value;
    }
}
