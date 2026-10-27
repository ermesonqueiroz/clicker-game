using Godot;
using System.Collections.Generic;

namespace Game.Common;

[Tool]
[GlobalClass]
public partial class SoundPool : Node
{
	private List<SoundQueue> _soundQueues = [];	
	private RandomNumberGenerator _random = new();
	private int _lastIndex = -1;

	public override void _Ready()
	{
		foreach (var child in GetChildren())
		{
			if (child is SoundQueue soundQueue)
			{
				_soundQueues.Add(soundQueue);
			}
		}
	}

	public override string[] _GetConfigurationWarnings()
	{
		int numberOfSoundQueues = 0;

		foreach (var child in GetChildren())
		{
			if (child is SoundQueue soundQueue)
			{
				_soundQueues.Add(soundQueue);
			}
		}

		if (numberOfSoundQueues < 2)
		{
			return ["Expected two or more children of type SoundQueue."];
		}

		return [];
	}

	public void PlayRandomSound()
	{
		int index;
		do
		{
			index = _random.RandiRange(0, _soundQueues.Count - 1);
		} while (index == _lastIndex);

		_lastIndex = index;
		_soundQueues[index].PlaySound();
	}
}
