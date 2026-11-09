using Godot;
using Game.Globals;
using Game.Resources;
using Godot.Collections;

namespace Game.Components;

[GlobalClass]
public partial class StatsComponent : Node
{
    [Export]
    private Dictionary<string, Stat> _stats = [];

    public override void _Ready()
    {
        UpgradeManager.Instance.CurrentUpgradeChanged += UpdateStats;
    }

    public Stat GetStat(string statName)
    {
        if (_stats.TryGetValue(statName, out Stat stat))
        {
            return stat;
        }
        else
        {
            GD.PrintErr($"Stat '{statName}' not found.");
            return null;
        }
    }

    public void UpdateStats()
    {
        foreach (Stat stat in _stats.Values)
        {
            stat?.Modifiers.Clear();
        }

        foreach (var entry in UpgradeManager.Instance.GetCurrentUpgrades())
        {
            BaseUpgrade baseUpgrade = entry.Key;
            Upgrade currentUpgrade = entry.Value;

            if (currentUpgrade == null)
            {
                continue;
            }

            for (Upgrade upgrade = baseUpgrade.Head; upgrade != null; upgrade = upgrade.Next)
            {
                foreach (Modifier modifier in upgrade.Modifiers)
                {
                    foreach (Stat stat in _stats.Values)
                    {
                        if (stat != null && modifier.Stat == stat)
                        {
                            stat.Modifiers.Add(modifier);
                        }
                    }
                }

                if (upgrade == currentUpgrade)
                {
                    break;
                }
            }
        }
    }
}
