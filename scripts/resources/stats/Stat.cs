using System;
using Godot;
using Game.Resources.Stats;
using System.Collections.Generic;
using System.Linq;

namespace Game.Resources.Stats;

[GlobalClass]
public partial class Stat : Resource
{
    [Export]
    public string ID = "";

    [Export]
    public double BaseValue;

    public Stat()
    {
    }

    public Stat(string id, double baseValue)
    {
        ID = id;
        BaseValue = baseValue;
    }

    public double Value
    {
        get
        {
            return _modifiers.Aggregate(BaseValue, (value, modifier) => modifier.Operation switch
            {
                Modifier.OperationType.Add => value + modifier.Value,
                Modifier.OperationType.Subtract => value - modifier.Value,
                Modifier.OperationType.Multiply => value * modifier.Value,
                Modifier.OperationType.Divide => value / modifier.Value,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(modifier.Operation),
                    modifier.Operation,
                    "Unsupported modifier operation.")
            });
        }   
    }

    private readonly List<Modifier> _modifiers = [];

    public void ApplyModifier(Modifier modifier)
    {
        _modifiers.Add(modifier);
    }
}
