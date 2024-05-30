using Godot;
using System;

public class TileManager
{
    int _tileSize = 16;
    public int TileSize { get { return _tileSize; } }
    public Vector2I TileSizeVector { get { return new Vector2I(_tileSize, _tileSize); } }
    public TileMap DungeonTM;


    public void Init()
    {
        DungeonTM = 
        Managers.Resource.Instantiate<TileMap>(
        Managers.Resource.LoadPackedScene<TileMap>(Define.Scenes.ContentNodes, "TileMap/dungeon_tile_map.tscn"),
        Managers.Instance);
    }
}


