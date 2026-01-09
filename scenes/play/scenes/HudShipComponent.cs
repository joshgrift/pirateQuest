using Godot;
using PiratesQuest.Data;
using PiratesQuest;

public partial class HudShipComponent : MarginContainer
{
	[Signal]
	public delegate void BuyButtonClickedEventHandler();

	[Signal]
	public delegate void EquipButtonClickedEventHandler();

	[Export] public Label ComponentNameLabel;
	[Export] public Label ComponentDescriptionLabel;
	[Export] public TextureRect ComponentIcon;
	[Export] public HBoxContainer CostList;
	[Export] public Button BuyButton;
	[Export] public Button EquipButton;

	// Called when the node enters the scene tree for the first time
	public override void _Ready()
	{
		BuyButton.Pressed += OnBuyButtonPressed;
		EquipButton.Pressed += OnEquipButtonPressed;
	}

	// Handler for buy button - emits our custom signal
	private void OnBuyButtonPressed()
	{
		EmitSignal(SignalName.BuyButtonClicked);
	}

	// Handler for equip button - emits our custom signal
	private void OnEquipButtonPressed()
	{
		EmitSignal(SignalName.EquipButtonClicked);
	}

	public void SetComponent(Component component, bool isOwned = false)
	{
		ComponentNameLabel.Text = component.name;
		ComponentDescriptionLabel.Text = component.description;

		// Set component icon
		Image img = component.icon.GetImage();
		img.Resize(100, 100);
		ImageTexture resizedTexture = ImageTexture.CreateFromImage(img);
		ComponentIcon.Texture = resizedTexture;

		if (isOwned)
		{
			BuyButton.Visible = false;
			EquipButton.Visible = true;
		}
		else
		{
			BuyButton.Visible = true;
			EquipButton.Visible = false;

			// Clear previous cost items
			foreach (Node child in CostList.GetChildren())
			{
				child.QueueFree();
			}

			// Add cost items with icons
			foreach (var costEntry in component.cost)
			{
				// Create container for each cost item (icon + label)
				HBoxContainer costItem = new HBoxContainer();

				// Add icon
				TextureRect iconRect = new TextureRect();
				iconRect.Texture = Icons.GetInventoryIcon(costEntry.Key);
				iconRect.CustomMinimumSize = new Vector2(24, 24);
				iconRect.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
				iconRect.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
				costItem.AddChild(iconRect);

				// Add cost label
				Label costLabel = new Label();
				costLabel.Text = costEntry.Value.ToString();
				costItem.AddChild(costLabel);

				CostList.AddChild(costItem);
			}
		}
	}
}
