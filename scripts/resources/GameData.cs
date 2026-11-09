using Godot;

[GlobalClass]
public partial class GameData : Resource
{
    [Signal]
    public delegate void BalanceChangedEventHandler(int finalAmount);

    private int _coinAmount;
    private int _pendingCoinAmount;

    [Export]
    public int CoinAmount
    {
        get => _coinAmount;
        set
        {
            if (_coinAmount != value)
            {
                _coinAmount = value;
                EmitSignal(SignalName.BalanceChanged, GetFinalAmount());
            }
        }
    }

    [Export]
    public int PendingCoinAmount
    {
        get => _pendingCoinAmount;
        set
        {
            if (_pendingCoinAmount != value)
            {
                _pendingCoinAmount = value;
                EmitSignal(SignalName.BalanceChanged, GetFinalAmount());
            }
        }
    }

    public int GetFinalAmount() => _coinAmount - _pendingCoinAmount;
}