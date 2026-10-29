using Godot;
using System;

namespace Game.Components;

public enum RoundPhase
{
	Intermission,
	Collecting
}

[GlobalClass]
public partial class RoundController : Node
{
	[Export]
	public float RoundDurationSeconds = 10.0f;

	public int RoundNumber { get; private set; }
	public float TimeRemaining { get; private set; }
	public RoundPhase Phase { get; private set; } = RoundPhase.Intermission;
	public bool IsCollecting => Phase == RoundPhase.Collecting;

	[Signal]
	public delegate void RoundStartedEventHandler(int roundNumber);

	[Signal]
	public delegate void TimeChangedEventHandler(int secondsRemaining);

	[Signal]
	public delegate void RoundEndedEventHandler(int roundNumber);

	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Pausable;

		if (!float.IsFinite(RoundDurationSeconds) || RoundDurationSeconds <= 0.0f)
		{
			throw new InvalidOperationException("Round duration must be finite and greater than zero.");
		}

		StartNextRound();
	}

	public override void _Process(double delta)
	{
		if (Phase != RoundPhase.Collecting)
		{
			return;
		}

		int previousDisplayedTime = Mathf.CeilToInt(TimeRemaining);
		TimeRemaining = Mathf.Max(TimeRemaining - (float)delta, 0.0f);
		int displayedTime = Mathf.CeilToInt(TimeRemaining);

		if (displayedTime != previousDisplayedTime)
		{
			EmitSignal(SignalName.TimeChanged, displayedTime);
		}

		if (TimeRemaining <= 0.0f)
		{
			EndRound();
		}
	}

	public void StartNextRound()
	{
		if (Phase == RoundPhase.Collecting)
		{
			return;
		}

		RoundNumber++;
		TimeRemaining = RoundDurationSeconds;
		Phase = RoundPhase.Collecting;

		EmitSignal(SignalName.RoundStarted, RoundNumber);
		EmitSignal(SignalName.TimeChanged, Mathf.CeilToInt(TimeRemaining));
	}

	private void EndRound()
	{
		Phase = RoundPhase.Intermission;
		TimeRemaining = 0.0f;
		EmitSignal(SignalName.TimeChanged, 0);
		EmitSignal(SignalName.RoundEnded, RoundNumber);
	}
}
