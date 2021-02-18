using System;
using System.Drawing;
using GXPEngine;

public class MyGame : Game
{
	Menu menu;

	Player player;
	public MyGame() : base(1280, 960, false, false)
	{
		targetFps = 60;

		menu = new Menu();
		AddChild(menu);
    }

    void Update()
	{

	}

	static void Main()
	{
		new MyGame().Start();
	}
}