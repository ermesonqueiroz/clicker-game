using Game.Globals;
using Game.Resources;
using Game.UI;
using Godot;
using System;

namespace Game.Screens.UpgradeScreen;

public partial class UpgradeButton : Control
{
	[Export]
	private Label _titleLabel;
	
	[Export]
	private Label _levelLabel;

	[Export]
	private Label _costLabel;

	[Export]
	private TextureRect _iconRect;

	[Export]
	private CustomButton _buyButton;

	private BaseUpgrade _baseUpgrade;
	private Upgrade _currentUpgrade;

	public void Initialize(BaseUpgrade baseUpgrade)
	{
		_baseUpgrade = baseUpgrade ?? throw new ArgumentNullException(nameof(baseUpgrade));

		_currentUpgrade = UpgradeManager.Instance.GetNextUpgrade(_baseUpgrade);
		if (_currentUpgrade == null)
		{
			throw new InvalidOperationException($"Base upgrade '{_baseUpgrade.Name}' has no next upgrade to display.");
		}

		_titleLabel.Text = _baseUpgrade.Name;
		_levelLabel.Text = $"Lv. {_currentUpgrade.Level}";
		_costLabel.Text = _currentUpgrade.Cost.ToString();
		
		if (_baseUpgrade.Icon != null)
		{
			_iconRect.Texture = _baseUpgrade.Icon;
		}

		GameManager.Instance.GameData.BalanceChanged += (_) => UpdateDisabled();
		UpdateDisabled();

		_buyButton.Pressed += OnBuyButtonPressed;
	}

	public void OnBuyButtonPressed()
	{
		Upgrade nextUpgrade = UpgradeManager.Instance.GetNextUpgrade(_baseUpgrade);
		if (nextUpgrade != null)
		{
			GameManager.Instance.GameData.CoinAmount -= _currentUpgrade.Cost;
			UpgradeManager.Instance.SetCurrentUpgrade(_baseUpgrade, nextUpgrade);
		}
	}

	public void UpdateDisabled()
	{
		if (GameManager.Instance.GameData.GetFinalAmount() < _currentUpgrade.Cost && !_buyButton.Disabled)
		{
			_buyButton.Disable();
			return;
		}

		if (GameManager.Instance.GameData.GetFinalAmount() >= _currentUpgrade.Cost && _buyButton.Disabled)
		{
			_buyButton.Enable();
		}
	}
}
