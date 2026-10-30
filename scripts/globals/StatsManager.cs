using System.Collections.Generic;
using Game.Resources.Stats;
using Godot;

namespace Game.Globals;

public partial class StatsManager : Node
{
	public static StatsManager Instance { get; private set; }

	public Dictionary<string, Stat> Stats { get; } = new()
	{
		{"idle_click_radius", new Stat("idle_click_radius", 20)}
	};

	public Dictionary<string, Modifier> Modifiers { get; } = new()
	{
		{"double_idle_click_radius", new Modifier(Callable.From((float stat) => stat * 2))}
	};

    public override void _Ready()
    {
        Instance = this;
    }

	public Stat GetStatByID(string ID)
	{
		return Stats[ID];
	}

	public Modifier GetModifierByID(string ID)
	{
		return Modifiers[ID];
	}
}
