using Godot;
using Game.Resources.Stats;
using System.Collections.Generic;
using System.Linq;

namespace Game.Resources.Stats;

[GlobalClass]
public partial class Stat : Resource
{
    [Export]
    public string ID;
    
    [Export]
    public double BaseValue;

    public double Value
    {
        get
        {
            return _modifiers.Aggregate(BaseValue, (acc, modifier) => (float)modifier.Formula.Call(acc));
        }   
    }

    private readonly List<Modifier> _modifiers = [];

    public void ApplyModifier(Modifier modifier)
    {
        _modifiers.Add(modifier);
    }
}
