using Algonquin1;
using Godot;
using System;

public partial class PortUi : PanelContainer
{
	[Export] public VBoxContainer StockListContainer;
	public Player Player;

	private PackedScene _shopItemScene = GD.Load<PackedScene>("res://scenes/port/shop_item.tscn");

	public override void _Ready()
	{
	}

	public void ChangeName(string name)
	{
		GetNode<Label>("MarginContainer/Port/PortName").Text = name;
	}

	public void SetStock(ShopItemData[] itemsForSale)
	{
		if (StockListContainer == null)
		{
			GD.PrintErr("StockListContainer is not assigned in the PortUi scene. Please assign it in the Inspector.");
			return;
		}

		foreach (Node child in StockListContainer.GetChildren())
		{
			child.QueueFree();
		}

		foreach (var item in itemsForSale)
		{

			var itemEntry = _shopItemScene.Instantiate<ShopItem>();
			itemEntry.ItemType = item.ItemType;
			itemEntry.BuyPrice = item.BuyPrice;
			itemEntry.SellPrice = item.SellPrice;

			itemEntry.TradeItem += (itemType, amount, price) =>
				{
					Player.UpdateInventory(itemType, amount, price);
				}
			;

			StockListContainer.AddChild(itemEntry);
		}
	}
}
