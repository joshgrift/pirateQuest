namespace Algonquin1;

using Godot;

public partial class ShopItem : HBoxContainer
{
	[Export] public InventoryItemType ItemType { get; set; } = InventoryItemType.Wood;
	[Export] public double BuyPrice { get; set; } = 0.0;
	[Export] public double SellPrice { get; set; } = 0.0;

	public override void _Ready()
	{
		var itemLabel = GetNode<Label>("ItemLabel");
		itemLabel.Text = ItemType.ToString();

		var buyLabel = GetNode<Label>("BuyPriceLabel");
		buyLabel.Text = $"Buy: -{BuyPrice:C}G";

		var sellLabel = GetNode<Label>("SellPriceLabel");
		sellLabel.Text = $"Sell: +{SellPrice:C}G";
	}
}
