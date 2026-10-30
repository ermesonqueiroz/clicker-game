using Game.Globals;
using Game.Resources.Skills;
using Godot;

namespace Game.UI;

public partial class SkillShop : CanvasLayer
{
	[Export]
	public GridContainer Container;

	public override void _Ready()
	{
		var slot = Container.GetChild<TextureButton>(0);

		foreach (Skill skill in SkillManager.Instance.AvailableSkills)
		{
			var newSlot = slot.Duplicate();
			newSlot.GetChild<Label>(0).Text = skill.Name;
			Container.CallDeferred("add_child", newSlot);
		}

		slot.QueueFree();
	}
}
