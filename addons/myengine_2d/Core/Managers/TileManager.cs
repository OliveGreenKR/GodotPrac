using Godot;
using System;

public class TileManager
{
    int _tileSize = 16;
    public int TileSize { get { return _tileSize; } }
    public Vector2I TileSizeVector { get { return new Vector2I(_tileSize, _tileSize); } }

}
