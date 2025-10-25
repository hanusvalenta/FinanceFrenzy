using Godot;

public partial class SpriteMoveUp : Node
{
	[Export] public CanvasItem nodeToWatch;
	[Export] public Sprite2D spriteToMove;
	[Export] public float moveSpeed = 50.0f;
	[Export] public float moveDistance = 100.0f;
	
	private bool wasVisible = false;
	private bool isMoving = false;
	private Vector2 startPosition;
	private Vector2 targetPosition;
	private float currentDistance = 0.0f;
	
	public override void _Ready()
	{
		if (spriteToMove != null)
		{
			startPosition = spriteToMove.Position;
		}
		
		if (nodeToWatch != null)
		{
			wasVisible = nodeToWatch.Visible;
		}
	}
	
	public override void _Process(double delta)
	{
		if (nodeToWatch == null || spriteToMove == null) return;
		
		// Check if visibility changed to true
		if (!wasVisible && nodeToWatch.Visible)
		{
			StartMovingUp();
		}
		
		// Also check if node just became visible from the start
		if (nodeToWatch.Visible && !isMoving && currentDistance == 0.0f)
		{
			StartMovingUp();
		}
		
		wasVisible = nodeToWatch.Visible;
		
		// Handle sprite movement
		if (isMoving)
		{
			float moveAmount = moveSpeed * (float)delta;
			currentDistance += moveAmount;
			
			if (currentDistance >= moveDistance)
			{
				currentDistance = moveDistance;
				isMoving = false;
			}
			
			float progress = currentDistance / moveDistance;
			spriteToMove.Position = startPosition.Lerp(targetPosition, progress);
		}
	}
	
	private void StartMovingUp()
	{
		if (spriteToMove == null || isMoving) return;
		
		startPosition = spriteToMove.Position;
		targetPosition = startPosition + new Vector2(0, -moveDistance);
		currentDistance = 0.0f;
		isMoving = true;
	}
	
	public void ResetPosition()
	{
		if (spriteToMove != null)
		{
			spriteToMove.Position = startPosition;
			isMoving = false;
			currentDistance = 0.0f;
		}
	}
}
