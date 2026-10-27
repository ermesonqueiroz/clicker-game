using Godot;
using System.Collections.Generic;

namespace Game.Common;

[GlobalClass]
public partial class SoundQueue : Node
{
	private int _next = 0;
	private List<AudioStreamPlayer> _audioStreamPlayers = [];

	[Export]
	public int Count { get; set; } = 1;

	public override void _Ready()
	{
		var child = GetChild(0);

		if (GetChildCount() == 0)
		{
			GD.Print("No AudioStreamPlayer child found");
			return;
		}

		if (child is AudioStreamPlayer audioPlayer)
		{
			_audioStreamPlayers.Add(audioPlayer);

			for (int i = 0; i < Count; i++)
			{
				var duplicate = audioPlayer.Duplicate() as AudioStreamPlayer;
				AddChild(duplicate);
				_audioStreamPlayers.Add(duplicate);
			}
		}
	}

	public override string[] _GetConfigurationWarnings()
	{
		if (GetChildCount() == 0)
		{
			return ["No children found. Expected one AudioStreamPlayer child."];
		}

		if (GetChild(0) is not AudioStreamPlayer)
		{
			return ["Expected first child to be an AudioStreamPlayer."];
		}

		return base._GetConfigurationWarnings();
	}

	public void PlaySound()
	{
		var player = _audioStreamPlayers[_next];

		if (!player.Playing)
		{
			player.Play();
			_next = (_next + 1) % _audioStreamPlayers.Count;
		}
	}
}
