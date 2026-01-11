namespace PiratesQuest;

using Godot;
using PiratesQuest.Data;
using PiratesQuest;

public enum HudShipComponentStatus
{
  ForSale,
  Owned,
  Equipped
}

public partial class HudShipComponent : MarginContainer
{
  [Signal]
  public delegate void ButtonClickedEventHandler();

  [Export] public Label ComponentNameLabel;
  [Export] public Label ComponentDescriptionLabel;
  [Export] public TextureRect ComponentIcon;
  [Export] public HBoxContainer CostList;
  [Export] public Button Button;

  public override void _Ready()
  {
    Button.Pressed += OnButtonPressed;
  }

  private void OnButtonPressed()
  {
    EmitSignal(SignalName.ButtonClicked);
  }

  public void SetComponent(Component component, HudShipComponentStatus status = HudShipComponentStatus.ForSale)
  {
    ComponentNameLabel.Text = component.name;
    ComponentDescriptionLabel.Text = component.description;

    // Set component icon
    Image img = component.icon.GetImage();
    img.Resize(100, 100);
    ImageTexture resizedTexture = ImageTexture.CreateFromImage(img);
    ComponentIcon.Texture = resizedTexture;

    switch (status)
    {
      case HudShipComponentStatus.Equipped:
        Button.Text = "Unequip";
        break;
      case HudShipComponentStatus.Owned:
        Button.Text = "Equip";
        break;
      case HudShipComponentStatus.ForSale:
        Button.Text = "Buy";
        break;
    }

    if (status == HudShipComponentStatus.ForSale)
    {
      foreach (Node child in CostList.GetChildren())
        child.QueueFree();

      foreach (var costEntry in component.cost)
      {
        HBoxContainer costItem = new();

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
