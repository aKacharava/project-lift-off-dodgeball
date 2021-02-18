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

    bool enableFade;

    public Button(string bText, int bX = 0, int bY = 0, int fontSize = 16) : base()
    {
        text = new EasyDraw(700, 25, false);
        text.x = bX;
        text.y = bY;
        text.TextFont(new Font("OCR A Extended", fontSize));
        text.TextAlign(CenterMode.Min, CenterMode.Min);
        AddChild(text);

        buttonText = bText;
    }

    void Update()
    {
        text.Clear(Color.Transparent);
        text.Text(buttonText, 0, 0);

        fadeText();
    }

    public void EnableFade(bool enable)
    {
        enableFade = enable;
    }

    public void fadeText()
    {
        if (enableFade == true)
        {
            text.alpha -= fadeSpeed;

            if (text.alpha == 0 || text.alpha == 1)
            {
                fadeSpeed = -fadeSpeed;
            }
        }
    }
}
