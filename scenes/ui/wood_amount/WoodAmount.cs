using Godot;
using Game.Globals;

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
        int startAmount = int.TryParse(CoinAmountLabel.Text, out int currentVal) ? currentVal : 0;
        int targetAmount = GameManager.Instance.GameData.GetFinalAmount();

        if (startAmount == targetAmount) return;

        var tween = CreateTween();
        tween.TweenMethod(
            Callable.From<float>(val => CoinAmountLabel.Text = Mathf.RoundToInt(val).ToString()),
            startAmount,
            (float)targetAmount,
            0.5f
        );
    }
}