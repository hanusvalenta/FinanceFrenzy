using Godot;

public partial class SimpleSpriteVisibilitySwitcher : Node
{
	[Export] public Node2D[] nodes = new Node2D[0];
	[Export] public float switchInterval = 2.0f;
	
	private int currentIndex = 0;
	private float timer = 0.0f;
	
	public override void _Ready()
	{
		if (nodes.Length > 0)
		{
			// Hide all nodes initially
			for (int i = 0; i < nodes.Length; i++)
			{
				nodes[i].Visible = false;
			}
			
			// Show the first node
			nodes[0].Visible = true;
		}
	}
	
	public override void _Process(double delta)
	{
		if (nodes.Length <= 1) return;
		
		timer += (float)delta;
		
		if (timer >= switchInterval)
		{
			timer = 0.0f;
			SwitchToNext();
		}
	}
	
	private void SwitchToNext()
	{
		// Hide current node
		nodes[currentIndex].Visible = false;
		
		// Move to next index
		currentIndex = (currentIndex + 1) % nodes.Length;
		
		// Show new current node
		nodes[currentIndex].Visible = true;
	}
}
