using Godot;
using System;

public partial class Test : Node2D
{
    static TileMap TM = Managers.Tile.DungeonTM;


    public override void _Ready()
	{
        Vector2 viewport =  GetViewport().GetVisibleRect().Size;
        GD.Print(viewport);

        var pos = GlobalPosition;
        var size = new Vector2I(10, 10) * TM.TileSet.TileSize.X;

        Room room = GenerateRoomAt(pos,size);

        var topleft = (room.GlobalPosition - room.Size/2).ToVector2I();
        var bottomright = (room.GlobalPosition + room.Size/2).ToVector2I();

        Godot.Collections.Array<Vector2I> cells = new Godot.Collections.Array<Vector2I>();

        for(int x = topleft.X; x < bottomright.X; x+=16)
        {
            for (int y = topleft.X; y < bottomright.X; y+= 16)
            {
                cells.Add(
                    TM.LocalToMap(ToLocal(new Vector2I(x, y)))
                    );
            }
        }
        GD.Print(cells.Count);
        TM.SetCellsTerrainConnect(1, cells, 0, 0);
  
	}

    public override void _Process(double delta)
    {
        
    }


    Room GenerateRoomAt(Godot.Vector2 position, Vector2I size)
    {
        Room room = Room.New(this);
        room.GlobalPosition = position;
        room.Size = size;
        return room;
    }

}
