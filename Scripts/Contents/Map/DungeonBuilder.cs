using Godot;
using Godot.Collections;
using System;
using static Godot.OpenXRInterface;


public partial class DungeonBuilder : Node
{
    public static Action DungeonCompleteAction = () => { };

    [Export]
    public int TileSize { get; set; } = 32;
    [Export]
    public Godot.Vector2 DuegonSize { get; set; } = new Godot.Vector2(200, 100);
    [Export]
    public int MaxRoomSize { get; set; } = 20;
    [Export]
    public int MinRoomSize { get; set; } = 5;

    public ulong? Seed { get; private set; }

    RandomNumberGenerator _rand= new RandomNumberGenerator();

    TileMap _tileMap;

    PackedScene _roomInstance;
    Dictionary<Define.RoomTypes, Node> _rooms = new Dictionary<Define.RoomTypes, Node>();

    public override void _Ready()
    {
        _rand.Seed = Seed ?? (ulong)DateTime.Now.Ticks;

        _tileMap = this.GetChildByType<TileMap>();
        _tileMap.RenderingQuadrantSize = TileSize;

        _roomInstance = Managers.Resource.LoadPackedScene<Room>(Define.Scenes.Nodes,"Map/room.tscn");
        
        //bind Define.RoomTypes to Node2D
        Bind();

        //todo: tilemap

        //test
        for (int i =0; i < 35; i++)
        {
            Godot.Vector2 pos = GetRandomPointInEllipse(DuegonSize) + GetViewport().GetVisibleRect().Size / 2;
            GenerateRoom(pos);
        }
        DungeonCompleteAction.Invoke();
    }

    public override void _Process(double delta)
    {
    }

    void Bind()
    {
        var type =  typeof(Define.RoomTypes);
        string[] names = Enum.GetNames(type);

        //Generate Room Node by Type
        for(int i = 0; i < names.Length; i++)
        {
            var name = names[i];  
            var child = this.GetorAddChildByName<Node2D>(name);
            child.Name = name;
            _rooms.Add((Define.RoomTypes)i,child);
        }
    }

    //generate Room
    void GenerateRoom(Godot.Vector2 position)
    {
        //--------[1]init Room  at point with random size
        Room room = Managers.Resource.Instantiate<Room>(_roomInstance, null);
        //room.Size = new Vector2I(_rand.RandiRange(MinRoomSize * TileSize, MaxRoomSize * TileSize),
        //                         _rand.RandiRange(MinRoomSize * TileSize, MaxRoomSize * TileSize));
        room.Size = new Vector2I(_rand.RandiRange(MinRoomSize , MaxRoomSize ),
                                 _rand.RandiRange(MinRoomSize , MaxRoomSize ));

        //--------[2]Select Main Rooms
        int standard = Mathf.FloorToInt(MinRoomSize + (MaxRoomSize - MinRoomSize) / 3f);
        if(room.Size.X > standard && room.Size.Y > standard)
        {
            //main rooms

        }


        room.Position = position;
        _rooms[room.RoomType].AddChild(room,true);
    }

    #region Math
    Godot.Vector2 GetRandomPointInCircle(float radius)
    {
        return GetRandomPointInEllipse( new Godot.Vector2(radius, radius));
    }

    Godot.Vector2 GetRandomPointInEllipse(Godot.Vector2 size)
    {
        float theta = 2 * Mathf.Pi * _rand.Randf();
        float u = _rand.Randf() + _rand.Randf();
        float r = u > 1 ? 2 - u : u;

        int x = RoundTileSize(size.X * r * Mathf.Cos(theta), TileSize);
        int y = RoundTileSize(size.Y * r * Mathf.Sin(theta), TileSize);
        return new Godot.Vector2(x, y);
    }

    /// <summary>
    /// round N to Multiples of M
    /// </summary>
    int RoundTileSize(float n, int tileSize)
    {
        return Mathf.FloorToInt(((n + tileSize - 1) / tileSize) * tileSize);
    }
    #endregion

}
