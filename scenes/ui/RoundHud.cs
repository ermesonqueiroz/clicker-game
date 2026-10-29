using Game.Components;
using Godot;

namespace Game.UI;

public partial class RoundHud : CanvasLayer
{
	[Export]
	private RoundController _roundController;

	[Export]
	private Label _roundLabel;

	[Export]
	private Control _roundEndedPanel;

	[Export]
	private Label _roundEndedLabel;

	[Export]
	private Button _nextRoundButton;

	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
		_roundController.TimeChanged += OnTimeChanged;
		_roundController.RoundStarted += OnRoundStarted;
		_roundController.RoundEnded += OnRoundEnded;
		_nextRoundButton.Pressed += OnNextRoundPressed;

		_roundEndedPanel.Hide();
		UpdateRoundLabel(_roundController.RoundNumber, Mathf.CeilToInt(_roundController.TimeRemaining));
	}

	private void OnTimeChanged(int secondsRemaining)
	{
		UpdateRoundLabel(_roundController.RoundNumber, secondsRemaining);
	}

	private void OnRoundStarted(int roundNumber)
	{
		_roundEndedPanel.Hide();
		UpdateRoundLabel(roundNumber, Mathf.CeilToInt(_roundController.TimeRemaining));
	}

	private void OnRoundEnded(int roundNumber)
	{
		_roundEndedLabel.Text = $"Rodada {roundNumber} concluída";
		_roundEndedPanel.Show();
	}

	private void OnNextRoundPressed()
	{
		_roundController.StartNextRound();
	}

	private void UpdateRoundLabel(int roundNumber, int secondsRemaining)
	{
		_roundLabel.Text = $"Rodada {roundNumber}  |  {secondsRemaining}s";
	}
}
