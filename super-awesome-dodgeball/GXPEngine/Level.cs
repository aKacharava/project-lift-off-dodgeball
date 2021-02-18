using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GXPEngine;
using TiledMapParser;

public class Level : GameObject
{
    public Level(string fileName)
    {
        String pathName = Path.GetDirectoryName(fileName);
        Map levelData = MapParser.ReadMap(fileName);
        PlaceImageLayer(levelData, pathName);
        SpawnTiles(levelData);
        SpawnObjects(levelData);
    }

    /// <summary>
    /// Add an image layer
    /// </summary>
    /// <param name="leveldata"></param>
    /// <param name="pathName"></param>
    private void PlaceImageLayer(Map leveldata, string pathName)
    {
        if (leveldata.ImageLayers == null || leveldata.ImageLayers.Length == 0)
            return;

        foreach (ImageLayer _imageLayer in leveldata.ImageLayers)
        {
            if (_imageLayer.Image == null)
                continue;

            string imageFilename = Path.Combine(pathName, _imageLayer.Image.FileName);
            AddChild(new BackgroundSprite(imageFilename));
        }
    }

    /// <summary>
    /// Spawns tiles to their positions
    /// </summary>
    /// <param name="leveldata"> This needs a leveldata map to read out of to get the data of the tiles </param>
    private void SpawnTiles(Map leveldata)
    {
        foreach (Layer _layer in leveldata.Layers)
        {
            if (leveldata.Layers == null || leveldata.Layers.Length == 0)
                continue;

            short[,] _tileNumbers = _layer.GetTileArray();

            for (int row = 0; row < _layer.Height; row++)
            {
                for (int col = 0; col < _layer.Width; col++)
                {
                    bool _isBackgroundTile = _layer.GetBoolProperty("Background");

                    int _tileNumber = _tileNumbers[col, row];
                    TileSet _tiles = leveldata.GetTileSet(_tileNumber);

                    string _filenameTiles = _tiles.Image.FileName;
                    _filenameTiles = _filenameTiles.Remove(0, 3);
                    if (_tileNumber > 0 && _isBackgroundTile == false)
                    {
                        CollisionTile _tile = new CollisionTile(_filenameTiles, _tiles.Columns, _tiles.Rows);
                        _tile.SetFrame(_tileNumber - _tiles.FirstGId);
                        _tile.x = col * _tile.width;
                        _tile.y = row * _tile.height;
                        AddChild(_tile);
                    }
                    else if (_tileNumber > 0 && _isBackgroundTile == true)
                    {
                        BackgroundTile _backgroundTile = new BackgroundTile(_filenameTiles, _tiles.Columns, _tiles.Rows);
                        _backgroundTile.SetFrame(_tileNumber - _tiles.FirstGId);
                        _backgroundTile.x = col * _backgroundTile.width;
                        _backgroundTile.y = row * _backgroundTile.height;
                        AddChild(_backgroundTile);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Spawns objects to their positions
    /// </summary>
    /// <param name="_leveldata"> This needs a leveldata map to read out of to get the data of the objects </param>
    private void SpawnObjects(Map leveldata)
    {
        if (leveldata.ObjectGroups == null || leveldata.ObjectGroups.Length == 0)
            return;

        ObjectGroup _objectGroup = leveldata.ObjectGroups[0];

        if (_objectGroup.Objects == null || _objectGroup.Objects.Length == 0)
            return;

        foreach (TiledObject obj in _objectGroup.Objects)
        {
            switch (obj.Name)
            {
                case "Player_1":
                    Player _player1 = new Player(obj.X, obj.Y, obj.Width, obj.Height);
                    AddChild(_player1);
                    break;
                case "Player_2":
                    Player _player2 = new Player(obj.X, obj.Y, obj.Width, obj.Height, Player.PlayerSelection.PLAYER_TWO);
                    AddChild(_player2);
                    break;
                case "Ball":
                    Ball _ball = new Ball(new Vec2(obj.X, obj.Y), obj.Width, obj.Height);
                    AddChild(_ball);
                    break;
            }
        }
    }
}