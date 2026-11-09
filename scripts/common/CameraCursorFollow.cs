using Godot;

public partial class CameraCursorFollow : Camera2D
{
	private const float MaxOffset = 30.0f;
	private const float FollowSpeed = 8.0f;
	private const float StopDistance = 0.05f;

	private Vector2 _initialPosition;

	public override void _Ready()
	{
		_initialPosition = GlobalPosition;
	}

	public override void _PhysicsProcess(double delta)
	{
		var viewport = GetViewport();
		var viewportSize = viewport.GetVisibleRect().Size;
		var mouseOffset = (viewport.GetMousePosition() - viewportSize * 0.5f) / (viewportSize * 0.5f);
		var targetPosition = _initialPosition + mouseOffset * MaxOffset;

		if (GlobalPosition.DistanceSquaredTo(targetPosition) <= StopDistance * StopDistance)
		{
			GlobalPosition = targetPosition;
			return;
		}

		var weight = (float)(1.0 - Mathf.Exp((float)(-FollowSpeed * delta)));
		GlobalPosition = GlobalPosition.Lerp(targetPosition, weight);
	}
}
