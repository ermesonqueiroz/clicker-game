using Game.Globals;
using Godot;

namespace Game.UI;

public partial class WoodAmount : Control
{
	[Export]
	public Label CoinAmountLabel;

    public override void _Ready()
    {
        GameManager.Instance.GameData.BalanceChanged += UpdateLabel;
		UpdateLabel(GameManager.Instance.GameData.GetFinalAmount());
    }


	public void UpdateLabel(int pendingCoinAmount)
	{
		CoinAmountLabel.Text = GameManager.Instance.GameData.GetFinalAmount().ToString();
	}
}
