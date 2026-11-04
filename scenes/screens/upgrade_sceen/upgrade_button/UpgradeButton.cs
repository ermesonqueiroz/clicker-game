using Game.Globals;
using Game.Resources;
using Godot;
using System;

namespace Game.Screens.UpgradeScreen;

public partial class UpgradeButton : Control
{
	[Export]
	public BaseUpgrade BaseUpgrade;

	[Export]
	private Label _titleLabel;
	
	[Export]
	private Label _levelLabel;

	[Export]
	private Label _costLabel;

	[Export]
	private Control _buyButton;

	public override void _Ready()
	{
		Upgrade upgrade	= UpgradeManager.Instance.GetNextUpgrade(BaseUpgrade);

		_titleLabel.Text = BaseUpgrade.Name;
		_levelLabel.Text = $"Lv. {upgrade.Level}";
		_costLabel.Text = upgrade.Cost.ToString();
		_buyButton.GuiInput += OnBuyButtonGuiInput;
	}

	public void OnBuyButtonGuiInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
		{
			UpgradeManager.Instance.SetCurrentUpgrade(
				BaseUpgrade, 
				UpgradeManager.Instance.GetNextUpgrade(BaseUpgrade)
			);
		}
	}
}
