using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

public class HUD : Canvas
{
    Player player;

    public HUD(Player pPlayer, int pX, int pY) : base(700, 64, false)
    {
        player = pPlayer;
        x = pX;
        y = pY;
    }

    void Update()
    {
        graphics.Clear(Color.Empty);

        if (player.state == Player.PlayerSelection.PLAYER_ONE)
        {
            graphics.DrawString("Player 1 Lives: " + player.GetLives(), new Font("OCR A Extended", 18), Brushes.White, 0, 0);
        }
        else if(player.state == Player.PlayerSelection.PLAYER_TWO)
        {
            graphics.DrawString("Player 2 Lives: " + player.GetLives(), new Font("OCR A Extended", 18), Brushes.White, 0, 0);
        }
    }
}