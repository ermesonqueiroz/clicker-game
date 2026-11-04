using Godot;
using Godot.Collections;

namespace Game.Resources;

[GlobalClass]
public partial class Stat(float BaseValue) : Resource
{
	[Export]
	public float BaseValue = BaseValue;

	[Export]
	public Array<Modifier> Modifiers = [];

    public float Value
    {
        get
        {
            float value = BaseValue;
            foreach (Modifier modifier in Modifiers)
            {
                switch (modifier.Operation)
                {
                    case Modifier.OperationType.Add:
                        value += modifier.Value;
                        break;
                    case Modifier.OperationType.Subtract:
                        value -= modifier.Value;
                        break;
                    case Modifier.OperationType.Multiply:
                        value *= modifier.Value;
                        break;
                    case Modifier.OperationType.Divide:
                        value /= modifier.Value;
                        break;
                }
            }
            return value;
        }
    }

	public Stat() : this(0) {}
}
