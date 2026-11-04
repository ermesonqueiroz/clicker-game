using Game.Globals;
using Game.Resources;
using Godot;
using System;
using System.Linq;

namespace Game.Screens.UpgradeScreen;

public partial class UpgradeScreen : Control
{
	[Export]
	private TextureButton _closeButton;

	[Export]
	private AnimationPlayer _animationPlayer;

	[Export]
	private Control _upgradesContainer;

	private PackedScene _upgradeButtonScene = GD.Load<PackedScene>("res://scenes/screens/upgrade_sceen/upgrade_button/upgrade_button.tscn");

	public override void _Ready()
	{
		_closeButton.Pressed += Exit;
		
		RenderBaseUpgrades();
		UpgradeManager.Instance.CurrentUpgradeChanged += RenderBaseUpgrades;
	}

	public void RenderBaseUpgrades()
	{
		foreach (Control child in _upgradesContainer.GetChildren().Cast<Control>())
		{
			child.QueueFree();
		}

		foreach (BaseUpgrade baseUpgrade in UpgradeManager.Instance.GetBaseUpgrades())
		{
			if (UpgradeManager.Instance.GetNextUpgrade(baseUpgrade) == null)
			{
				continue;
			}

			UpgradeButton upgradeButton = _upgradeButtonScene.Instantiate<UpgradeButton>();
			upgradeButton.BaseUpgrade = baseUpgrade;
			_upgradesContainer.AddChild(upgradeButton);
		}
	}

	public void Enter()
	{
		CursorManager.Instance.DisableIdleCursor();
		_animationPlayer.Play("enter");
	}

	public void Exit()
	{
		CursorManager.Instance.EnableIdleCursor();
		_animationPlayer.Play("exit");
	}
}
