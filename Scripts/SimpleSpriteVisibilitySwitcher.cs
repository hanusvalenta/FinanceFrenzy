using Godot;

public partial class SimpleSpriteVisibilitySwitcher : Node2D
{
	[Export] public Node2D[] nodes = new Node2D[3];
	[Export] public float switchInterval = 2.0f;
	
	private int currentIndex = 0;
	private double timer = 0.0f;

	public override void _Ready()
	{GD.Print("Any");
		timer = switchInterval;
	}
	
	public override void _Process(double delta)
	{
		if(nodes.Length > 1 && currentIndex < nodes.Length)
		{
			timer = timer - delta;
		
			if(timer < 0)
			{
				timer = switchInterval;
				SwitchToNext();
			}
		}
	}
	
	private void SwitchToNext()
	{
		// Hide current node
		nodes[currentIndex].Visible = false;
		
		// Move to next index
		currentIndex++;
		
		// Show new current node
		nodes[currentIndex].Visible = true;
	}
}
