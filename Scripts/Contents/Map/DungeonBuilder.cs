using Godot;
using Godot.Collections;
using System;
using DelaunatorSharp;
using System.Linq;
using System.Threading.Tasks;

public partial class DungeonBuilder : Node
{
    public static Action DungeonCompleteAction = () => { };

    [Export]
    public int TileSize { get; set; }
    [Export]
    public Godot.Vector2I DuegonSize { get; set; } = new Godot.Vector2I(200, 100);
    [Export]
    public int RoomCount { get; set; }
    [Export]
    public int MaxRoomSize { get; set; }
    [Export]
    public int MinRoomSize { get; set; }

    public ulong? Seed { get; private set; }

    //random gernerator
    RandomNumberGenerator _rand = new RandomNumberGenerator();

    #region Tile &$ Deluaunry
    TileMap _tileMap;
    Delaunator _delaunator;

    #endregion
    //room
    PackedScene _roomInstance;
    Godot.Collections.Dictionary<Define.RoomTypes, Node> _TypeRooms = new Godot.Collections.Dictionary<Define.RoomTypes, Node>();

    public override void _Ready()
    {
        _rand.Seed = Seed ?? (ulong)DateTime.Now.Ticks;

        _tileMap = this.GetChildByType<TileMap>();
        _tileMap.RenderingQuadrantSize = TileSize;
        _roomInstance = Managers.Resource.LoadPackedScene<Room>(Define.Scenes.Nodes, "Map/room.tscn");

        //generate Node2D each to 'Define.RoomTypes'
        Bind();
        GenerateDungeon();
        //todp :tilemap
    }

    public override void _Process(double delta)
    {
    }

    void Bind()
    {
        var type = typeof(Define.RoomTypes);
        string[] names = Enum.GetNames(type);

        //Generate Room Node by Type
        for (int i = 0; i < names.Length; i++)
        {
            var name = names[i];
            var child = this.GetorAddChildByName<Node>(name);
            child.Name = name;
            _TypeRooms.Add((Define.RoomTypes)i, child);
        }
    }

    public async  void GenerateDungeon()
    {
        Array<Room> tmpRooms = new Array<Room>();
        //generating room
        for (int i = 0; i < RoomCount; i++)
        {
            Godot.Vector2I pos = GetRandomPointInEllipse(DuegonSize) + GetViewport().GetVisibleRect().Size.ToVector2I() / 2;
            var tmp = GenerateRoomRandomlyAt(pos);
            tmpRooms.Add(tmp);
            _TypeRooms[tmp.RoomType].AddChild(tmp, true);
        }
        //select main rooms
        float standard = new Vector2I(MinRoomSize * TileSize, MaxRoomSize * TileSize).Length() * 1f;
        Array<Room> selected =  await SelectMainRoomsAsync(tmpRooms, standard);

        //delaunary main Rooms
        Define.GridPoint[] points = selected.Select(room => room.GlobalPosition.ToVector2I().ToGridPoint()).ToArray();

        _delaunator = new Delaunator(points);
        GD.Print($"{_delaunator.GetTriangles().Count()}");
        Line2D drawer = this.GetChildByType<Line2D>();
        foreach (DelaunatorSharp.Triangle tri in _delaunator.GetTriangles())
        {
            _delaunator.ForEachTriangleEdge((IEdge edge) =>
            {
                //draw line p-q
                var p1 = new Vector2((int)edge.P.X, (int)edge.P.Y);
                var p2 = new Vector2((int)edge.Q.X, (int)edge.Q.Y);
                drawer.AddPoint(p1);
                drawer.AddPoint(p2);
            });
        }

        //dugeon build fin
        DungeonCompleteAction.Invoke();
    }

    //generate Room
    Room GenerateRoomRandomlyAt(Godot.Vector2 position)
    {
        //--------[1]init Room  at point with random size
        Room room = Managers.Resource.Instantiate<Room>(_roomInstance, null);
        room.Size = new Vector2I(_rand.RandiRange(MinRoomSize * TileSize, MaxRoomSize * TileSize),
                                 _rand.RandiRange(MinRoomSize * TileSize, MaxRoomSize * TileSize));
        room.GlobalPosition = position;
        return room;
    }

    async Task<Array<Room>> SelectMainRoomsAsync(Array<Room> rooms, float standard)
    {
        Array<Room> selected = SelectMainRooms(rooms, standard);
        int i = 0;
        while( selected.Count < 3)
        {
            standard *= 0.9f;
            selected = SelectMainRooms(rooms, standard);
            if(i % 100 == 0)
            {
                await ToSignal(GetTree().CreateTimer(0.3f), Timer.SignalName.Timeout);
            }
        }
        return selected;
    }

    static Array<Room> SelectMainRooms(Array<Room> rooms, float standard)
    {
        Array<Room> selected = new Array<Room>();

        if (rooms == null)
        {
            GD.PushWarning("Can not Slecting Main Rooms.");
            return null;
        }

        selected.AddRange(rooms.OrderBy(room => room.Size.Length())
            .Where(room => room.Size.Length() > standard)
            .Select((room) =>
            {
                room.GetChildByType<CollisionShape2D>().DebugColor = Color.FromHtml("db56576b");
                return room;
            }));

        return selected;
    }




    #region Math
    Godot.Vector2I GetRandomPointInCircle(int radius)
    {
        return GetRandomPointInEllipse(new Godot.Vector2I(radius, radius));
    }

    Godot.Vector2I GetRandomPointInEllipse(Godot.Vector2I size)
    {
        float theta = 2 * Mathf.Pi * _rand.Randf();
        float u = _rand.Randf() + _rand.Randf();
        float r = u > 1 ? 2 - u : u;

        int x = RoundTileSize(size.X * r * Mathf.Cos(theta), TileSize);
        int y = RoundTileSize(size.Y * r * Mathf.Sin(theta), TileSize);
        return new Godot.Vector2I(x, y);
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

