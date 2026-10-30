using System;
using System.Collections.Generic;
using Game.Globals;
using Game.Resources.Skills;
using Game.Resources.Stats;
using Godot;

namespace Game.Components;

[GlobalClass]
public partial class StatsComponent : Node
{
	public const string GroupName = "stats_components";

	[Export]
	public Godot.Collections.Array<Stat> Stats = [];

	private readonly Dictionary<string, Stat> _stats = new(StringComparer.Ordinal);
	private readonly HashSet<string> _appliedSkillIds = new(StringComparer.Ordinal);

	public override void _Ready()
	{
		foreach (Stat configuredStat in Stats)
		{
			if (configuredStat == null || string.IsNullOrWhiteSpace(configuredStat.ID))
			{
				GD.PushError("StatsComponent contains a stat with no ID.");
				continue;
			}

			Stat stat = (Stat)configuredStat.Duplicate(true);
			if (!_stats.TryAdd(stat.ID, stat))
			{
				GD.PushError($"StatsComponent contains duplicate stat ID '{stat.ID}'.");
			}
		}

		AddToGroup(GroupName);
		SkillManager.Instance.RestoreSkills(this);
	}

	public Stat GetStatByID(string id)
	{
		if (!_stats.TryGetValue(id, out Stat stat))
		{
			throw new KeyNotFoundException($"Stat '{id}' is not configured on this StatsComponent.");
		}

		return stat;
	}

	public void ApplySkill(Skill skill)
	{
		if (skill == null || string.IsNullOrWhiteSpace(skill.ID))
		{
			GD.PushError("Cannot apply a skill with no ID.");
			return;
		}

		if (!_appliedSkillIds.Add(skill.ID))
		{
			return;
		}

		foreach (Modifier modifier in skill.Modifiers)
		{
			if (modifier == null || string.IsNullOrWhiteSpace(modifier.StatID))
			{
				GD.PushError($"Skill '{skill.ID}' contains a modifier with no stat ID.");
				continue;
			}

			if (_stats.TryGetValue(modifier.StatID, out Stat stat))
			{
				stat.ApplyModifier(modifier);
			}
		}
	}
}
