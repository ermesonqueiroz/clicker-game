using Godot;

[GlobalClass]
public partial class GameData : Resource
{
    [Signal]
    public delegate void BalanceChangedEventHandler(int finalAmount);

    [Signal]
    public delegate void LumberjackAmountChangedEventHandler(int amount);

    private int _coinAmount;

    private int _lumberjackAmount;

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
    public int LumberjackAmount
    {
        get => _lumberjackAmount;
        set
        {
            if (_lumberjackAmount != value)
            {
                _lumberjackAmount = value;
                EmitSignal(SignalName.LumberjackAmountChanged, _lumberjackAmount);
            }
        }
    }

    public int GetFinalAmount() => _coinAmount;
}