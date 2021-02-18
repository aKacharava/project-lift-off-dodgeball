using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

public class Button : GameObject
{
    EasyDraw text;

    float fadeSpeed = 0.02f;
    string buttonText;

    public Button(string bText, int bX = 0, int bY = 0) : base()
    {
        text = new EasyDraw(700, 25, false);
        text.x = bX;
        text.y = bY;
        text.TextFont(new Font("bahnschrift", 16));
        text.TextAlign(CenterMode.Min, CenterMode.Min);
        AddChild(text);

        buttonText = bText;
    }

    void Update()
    {
        text.Clear(Color.Transparent);
        text.Text(buttonText, 0, 0);

        text.alpha -= fadeSpeed;

        if (text.alpha == 0 || text.alpha == 1)
        {
            fadeSpeed = -fadeSpeed;
        }
    }
}
