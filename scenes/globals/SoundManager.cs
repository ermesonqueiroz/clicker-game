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

	[Export]
	private SoundPool _woodStackSoundPool;

	[Export]
	private SoundPool _popStackSoundPool;

	public override void _Ready()
	{
		Instance = this;

		_soundPools["ChoppingWood"] = _choppingWoodSoundPool;
		_soundPools["WoodStack"] = _woodStackSoundPool;
		_soundPools["Pop"] = _popStackSoundPool;
	}

	public void PlayChoppingWoodSound()
	{
		_soundPools["ChoppingWood"].PlayRandomSound();
	}

	public void PlaWoodStackSound()
	{
		_soundPools["WoodStack"].PlayRandomSound();
	}

	public void PlayPopSound()
	{
		_soundPools["Pop"].PlayRandomSound();
	}
}
