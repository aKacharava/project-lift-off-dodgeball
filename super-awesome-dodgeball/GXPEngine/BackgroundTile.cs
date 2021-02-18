using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

public class BackgroundTile : AnimationSprite
{
    public BackgroundTile(string filename, int cols, int rows) : base(filename, cols, rows, addCollider: false)
    {

    }
}