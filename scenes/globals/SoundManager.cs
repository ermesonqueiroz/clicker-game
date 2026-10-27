using Godot;
using System.Collections.Generic;
using Game.Common;

namespace Game.Globals;

public partial class SoundManager : Node
{
	public static SoundManager Instance { get; private set; }
	private readonly Dictionary<string, SoundPool> _soundPools = [];

	[Export]
	private SoundPool _choppingWoodSoundPool;

	public override void _Ready()
	{
		Instance = this;

		_soundPools["ChoppingWood"] = _choppingWoodSoundPool;
	}

	public void PlayChoppingWoodSound()
	{
		_soundPools["ChoppingWood"].PlayRandomSound();
	}
}
