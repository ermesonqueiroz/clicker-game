using System;
using System.Collections.Generic;
using Game.Globals;
using Game.Resources.Stats;
using Godot;

namespace Game.Common.DevConsole;

[GlobalClass]
public partial class DevConsole : CanvasLayer
{
	[Export]
	private LineEdit _lineEdit;

	[Export]
	private RichTextLabel _output;

	private readonly Dictionary<string, Func<string[], string>> _commands = new(StringComparer.OrdinalIgnoreCase);
	private Expression _expression = new();

	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
		Visible = false;
		_lineEdit.TextSubmitted += OnTextSubmitted;
		_output.BbcodeEnabled = false;
		RegisterCommand("help", _ => $"Available commands: {string.Join(", ", _commands.Keys)}");
		RegisterCommand("echo", args => string.Join(" ", args));
		RegisterCommand("apply_modifier", ApplyModifierCommand);
		AppendOutput("Developer console. Type 'help' to list commands.");
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is not InputEventKey keyEvent || !keyEvent.Pressed || keyEvent.Echo)
		{
			return;
		}

		if (keyEvent.Keycode == Key.F1)
		{
			SetConsoleVisible(!Visible);
			GetViewport().SetInputAsHandled();
			return;
		}

		if (Visible && keyEvent.Keycode == Key.Escape)
		{
			SetConsoleVisible(false);
			GetViewport().SetInputAsHandled();
		}
	}

	public void OnTextSubmitted(string command)
	{
		command = command.Trim();
		_lineEdit.Clear();
		if (command.Length == 0)
		{
			return;
		}

		AppendOutput($"> {command}");

		string[] parts = command.Split(
			' ',
			StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
		);

		if (_commands.TryGetValue(parts[0], out Func<string[], string> handler))
		{
			try
			{
				string commandResult = handler(parts[1..]);
				AppendOutput(commandResult);
			}
			catch (ArgumentException exception)
			{
				AppendOutput($"Invalid command usage: {exception.Message}");
			}

			_lineEdit.GrabFocus();
			return;
		}

		Error error = _expression.Parse(command);
		if (error != Error.Ok)
		{
			AppendOutput($"Parse error: {_expression.GetErrorText()}");
			_lineEdit.GrabFocus();
			return;
		}

		Variant result = _expression.Execute(new Godot.Collections.Array(), this);
		if (_expression.HasExecuteFailed())
		{
			AppendOutput("Expression execution failed. See the Output panel for details.");
			_lineEdit.GrabFocus();
			return;
		}

		AppendOutput(result.ToString());
		_lineEdit.GrabFocus();
	}

	public void RegisterCommand(string name, Func<string[], string> handler)
	{
		if (string.IsNullOrWhiteSpace(name) || name.Contains(' '))
		{
			throw new ArgumentException("Command names must be non-empty and contain no spaces.", nameof(name));
		}

		ArgumentNullException.ThrowIfNull(handler);
		_commands[name] = handler;
	}

	private void SetConsoleVisible(bool visible)
	{
		GetTree().Paused = visible;
		Visible = visible;
		if (visible)
		{
			_lineEdit.GrabFocus();
		}
		else
		{

			_lineEdit.ReleaseFocus();
		}
	}

	private void AppendOutput(string message)
	{
		_output.AppendText($"{message}\n");
		_output.ScrollToLine(_output.GetLineCount() - 1);
	}

	private string ApplyModifierCommand(string[] args)
	{
		if (args[0] is null)
		{
			return "Invalid Stat ID.";
		}

		if (args[1] is null)
		{
			return "Invalid Modifier ID.";
		}

		Stat stat = StatsManager.Instance.GetStatByID(args[0]);

		if (stat == null)
		{
			return "Stat not found.";
		}

		Modifier modifier = StatsManager.Instance.GetModifierByID(args[1]);

		if (stat == null)
		{
			return "Modifier not found.";
		}

		stat.ApplyModifier(modifier);

		return "Modifier applied successfully!";
	}
}
