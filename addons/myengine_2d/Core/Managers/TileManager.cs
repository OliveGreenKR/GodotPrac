using Godot;
using System;

public class TileManager
{
    public int TileSize { get; private set; }
    public Vector2I TileSizeVector { get; private set; }
    public TileMap DungeonTM;


    public void Init()
    {
        DungeonTM = 
        Managers.Resource.Instantiate<TileMap>(
        Managers.Resource.LoadPackedScene<TileMap>(Define.Scenes.ContentNodes, "TileMap/dungeon_tile_map.tscn"),
        Managers.Instance);
        //DungeonTM.Name = "DungeonTM";
        TileSize = DungeonTM.TileSet.TileSize.X;
        TileSizeVector = DungeonTM.TileSet.TileSize;
        DungeonTM.RenderingQuadrantSize = TileSize;
    }
}


