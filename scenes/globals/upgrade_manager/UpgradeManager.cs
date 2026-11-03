using Godot;
using Godot.Collections;
using Game.Resources;

namespace Game.Globals;

[GlobalClass]
public partial class UpgradeManager : Node
{
	public UpgradeManager Instance { get; private set; }

	[Export]
	public Array<Upgrade> Upgrades = [];

	public override void _Ready()
	{
		Instance = this;
	}
}
