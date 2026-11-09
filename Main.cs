using Game.Globals;
using Game.Screens.UpgradeScreen;
using Game.UI;
using Godot;

public partial class Main : Node2D
{
	[Export]
	public Node UiContainer;

	[Export]
	private CustomButton _upgradeScreenButton;

	[Export]
	private UpgradeScreen _upgradeScreen;

	public override void _Ready()
	{
		GameManager.Instance.UiContainer = UiContainer;
		_upgradeScreenButton.Pressed += OnUpgradeScreenButtonPressed;
	}

	public void OnUpgradeScreenButtonPressed()
	{
		_upgradeScreen.Enter();
	}
}
