using Godot;
using Godot.Collections;
using Game.Resources;

namespace Game.Globals;

[GlobalClass]
public partial class UpgradeManager : Node
{
	public static UpgradeManager Instance { get; private set; }

	[Signal]
	public delegate void CurrentUpgradeChangedEventHandler();

	[Export]
	public Array<BaseUpgrade> AvailableUpgrades = [];

	private Dictionary<BaseUpgrade, Upgrade> _upgrades = [];

	public override void _Ready()
	{
		Instance = this;

		foreach (BaseUpgrade baseUpgrade in AvailableUpgrades)
		{
			_upgrades.Add(baseUpgrade, null);
		}
	}

	public Array<BaseUpgrade> GetBaseUpgrades()
	{
		return AvailableUpgrades;
	}

	public Upgrade GetCurrentUpgrade(BaseUpgrade baseUpgrade)
	{
		return _upgrades.TryGetValue(baseUpgrade, out Upgrade value) ? value : null;
	}

	public Upgrade GetNextUpgrade(BaseUpgrade baseUpgrade)
	{
		_upgrades.TryGetValue(baseUpgrade, out Upgrade value);

		if (value == null)
		{
			return baseUpgrade.Head;
		}

		return value.Next;
	}

	public void SetCurrentUpgrade(BaseUpgrade baseUpgrade, Upgrade upgrade)
	{
		_upgrades[baseUpgrade] = upgrade;
		EmitSignal(SignalName.CurrentUpgradeChanged);
	}

	public Dictionary<BaseUpgrade, Upgrade> GetCurrentUpgrades()
	{
		return _upgrades;
	}
}
