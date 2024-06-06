using Godot;
using System;

public partial class Test : Node2D
{
    static TileMap TM = Managers.Tile.DungeonTM;

    int GenerateRoomCount = 40;
    Vector2I DungeonSize =  new Vector2I(500,500);
    Vector2I minRoomSize =  new Vector2I(4,5);
    Vector2I maxRoomSize =  new Vector2I(12,12);
    public override void _Ready()
	{

        //generating room
        for (int i = 0; i < 3; i++)
        {
            Vector2 pos = Utils.GetRandomPointInEllipse(DungeonSize);
            var size = Managers.Random.RandVector(minRoomSize, maxRoomSize) * Managers.Tile.TileSize;
            var room = GenerateRoomAt(pos,size.ToVector2I());
        }

  
	}

    public override void _Process(double delta)
    {
        
    }

    void TilingRoom(Room room)
    {
        var topleft = (room.GlobalPosition - room.Size / 2).ToVector2I();
        var bottomright = (room.GlobalPosition + room.Size / 2).ToVector2I();

        Godot.Collections.Array<Vector2I> cells = new Godot.Collections.Array<Vector2I>();

        for (int x = topleft.X; x < bottomright.X; x += Managers.Tile.TileSize)
        {
            for (int y = topleft.X; y < bottomright.X; y += Managers.Tile.TileSize)
            {
                cells.Add(
                    TM.LocalToMap(ToLocal(new Vector2I(x, y)))
                    );
            }
        }

        TM.SetCellsTerrainConnect(1, cells, 0, 0);
    }




    Room GenerateRoomAt(Godot.Vector2 position, Vector2I size)
    {
        //Room room = Room.New(this);
        Room room = new Room();
        room.GlobalPosition = position;
        room.Size = size;

        AddChild(room,true);
        return room;
    }

}
