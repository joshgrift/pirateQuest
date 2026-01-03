using Godot;
using System;

public partial class PortUi : PanelContainer
{
	public override void _Ready()
	{
	}

	public void ChangeName(string name)
	{
		GetNode<Label>("MarginContainer/Port/PortName").Text = name;
	}
}
