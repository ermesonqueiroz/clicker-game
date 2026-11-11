using Game.Globals;
using Game.UI;
using Godot;

public partial class LumberjackButton : Control
{
	[Export]
	private CustomButton _buyButton;

	[Export]
	private Label _costLabel;

	[Export]
	private Label _quantityLabel;


	private int _currentCost => Mathf.RoundToInt(10 * Mathf.Pow(1.15, GameManager.Instance.GameData.LumberjackAmount));

	public override void _Ready()
	{
		GameManager.Instance.GameData.LumberjackAmountChanged += (_) => UpdateCostLabel();
		_buyButton.Pressed += OnBuyButtonPressed;
		GameManager.Instance.GameData.BalanceChanged += (_) => UpdateDisabled();

		UpdateDisabled();
		UpdateCostLabel();
	}

	public void OnBuyButtonPressed()
	{
		int cost = _currentCost;
		if (GameManager.Instance.GameData.GetFinalAmount() < cost)
		{
			UpdateDisabled();
			return;
		}

		GameManager.Instance.GameData.CoinAmount -= cost;
		GameManager.Instance.GameData.LumberjackAmount += 1;

		UpdateDisabled();
		UpdateCostLabel();
	}

	public void UpdateCostLabel()
	{
		_costLabel.Text = _currentCost.ToString();
		_quantityLabel.Text = GameManager.Instance.GameData.LumberjackAmount.ToString();
	}

	public void UpdateDisabled()
	{
		if (GameManager.Instance.GameData.GetFinalAmount() < _currentCost && !_buyButton.Disabled)
		{
			_buyButton.Disable();
			return;
		}

		if (GameManager.Instance.GameData.GetFinalAmount() >= _currentCost && _buyButton.Disabled)
		{
			_buyButton.Enable();
		}
	}
}
