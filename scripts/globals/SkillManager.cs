using Godot;
using Godot.Collections;
using Game.Resources.Skills;
using Game.Resources;
using Game.Components;
using System;
using System.IO;

namespace Game.Globals;

public partial class SkillManager : Node
{
	public static SkillManager Instance { get; private set; }

	public const string SkillResourcesPath = "res://resources/skills/";

	public Array<Skill> AvailableSkills = [];

	public override void _Ready()
	{
		Instance = this;

		var dir = DirAccess.Open(SkillResourcesPath);
		if (dir == null)
		{
			GD.PushError("Skill resources path not found: " + SkillResourcesPath);
			return;
		}

		dir.ListDirBegin();
		string fileName = dir.GetNext();
        while (fileName != "")
        {
            if (!dir.CurrentIsDir() && Path.GetExtension(fileName).Equals(".tres", StringComparison.OrdinalIgnoreCase))
            {
				var skill = GD.Load<Skill>(SkillResourcesPath + fileName);
        		if (skill == null)
        		{
        			GD.PushError("Failed to load skill resource: " + SkillResourcesPath + fileName);
        		}
        		else
        		{
        			AvailableSkills.Add(skill);
        		}
            }

            fileName = dir.GetNext();
        }
        dir.ListDirEnd();
	}

	public Skill GetSkillByID(string ID)
	{
		Skill foundSkill = null;

		foreach (Skill skill in AvailableSkills)
		{
			if (skill.ID == ID)
			{
				foundSkill = skill;
			}	
		}

		return foundSkill;
	}

	public bool ApplySkill(Skill skill)
	{
		if (skill == null || string.IsNullOrWhiteSpace(skill.ID))
		{
			GD.PushError("Cannot unlock a skill with no ID.");
			return false;
		}

		SaveGame saveGame = GameManager.Instance.SaveGame;
		foreach (Skill unlockedSkill in saveGame.UnlockedSkills)
		{
			if (unlockedSkill?.ID == skill.ID)
			{
				return false;
			}
		}

		saveGame.UnlockedSkills.Add(skill);
		foreach (Node node in GetTree().GetNodesInGroup(StatsComponent.GroupName))
		{
			if (node is StatsComponent statsComponent)
			{
				statsComponent.ApplySkill(skill);
			}
		}

		return true;
	}

	public void RestoreSkills(StatsComponent statsComponent)
	{
		foreach (Skill skill in GameManager.Instance.SaveGame.UnlockedSkills)
		{
			statsComponent.ApplySkill(skill);
		}
	}
}
