using Godot;
using Godot.Collections;
using System;
using DelaunatorSharp;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using MEC;

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
    public int MinMainRoomCount { get; set; }
    [Export]
    public int MaxRoomSize { get; set; }
    [Export]
    public int MinRoomSize { get; set; }

    public ulong? Seed { get; private set; }

    //random gernerator
    RandomNumberGenerator _rand = new RandomNumberGenerator();

    #region Tile &$ Deluaunry
    TileMap _tileMap;

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

        Task.Run(() => { GenerateDungeon(); });
        //todp :tilemap
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

    public async void GenerateDungeon()
    {
        await this.WaitForSeconds(GetPhysicsProcessDeltaTime(),processInPhysics:true);

        List<Room> tmpRooms = new List<Room>();
        Delaunator delaunator;
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

        List<Room> selected = SelectMainRooms(tmpRooms, standard);
        
        //wait for positioning Rooms
        await this.WaitForSeconds(1f, processInPhysics: true);

        //delaunary main Rooms
        //Define.GridPoint[] points = selected.Select( room =>  room.GlobalPosition.ToVector2I().ToGridPoint() ).ToArray();
        Define.GridPoint[] points = selected.Select((room, i) =>
        { return new Define.GridPoint { Vector = room.GlobalPosition.ToVector2I(), Index = i }; }
        ).ToArray();

        delaunator = new Delaunator(points);

        //1. adjacent list
        System.Collections.Generic.Dictionary<int, List<int>> visited =  new System.Collections.Generic.Dictionary<int, List<int>>();
        PriorityQueue<Define.GridPoint, int> pq = new PriorityQueue<Define.GridPoint, int>();

        foreach (IEdge edge in delaunator.GetEdges())
        {
            var idx = (edge.P as Define.GridPoint).Index;
            GD.Print(idx);
        }

        //todo :make edge to road.
        
        //draw triange
        DrawTriangles(delaunator);


        //dugeon build finished
        DungeonCompleteAction.Invoke();
    }

    void DrawTriangles( Delaunator delaunator)
    {
        Line2D drawer = this.GetOrAddChildByType<Line2D>();
        foreach (Triangle tri in delaunator.GetTriangles())
        {
            delaunator.ForEachTriangleEdge((IEdge edge) =>
            {
                if (edge.Index > delaunator.Halfedges[edge.Index])
                {
                    //draw line p-q
                    drawer.AddPoint(edge.P.ToVector2());
                    drawer.AddPoint(edge.Q.ToVector2());
                }
            });
        }
    }

    //generate Single Room
    Room GenerateRoomRandomlyAt(Godot.Vector2 position)
    {
        //--------[1]init Room  at point with random size
        Room room = Managers.Resource.Instantiate<Room>(_roomInstance,null);
        room.Size = new Vector2I(_rand.RandiRange(MinRoomSize * TileSize, MaxRoomSize * TileSize),
                                 _rand.RandiRange(MinRoomSize * TileSize, MaxRoomSize * TileSize));
        room.Position = position;
        return room;
    }

    List<Room> SelectMainRooms(List<Room> rooms, float standard)
    {
        if (rooms == null)
        {
            GD.PushWarning("Can not Slecting Main Rooms.");
            return null;
        }

        //divide room with Size
        var groupRoom = rooms.GroupBy(room => room.Size.Length() > standard)
            .ToDictionary(g => g.Key , g => g.ToList());

        var selected = groupRoom[true];

        //fill in the shortfall
        if (groupRoom[true].Count < MinMainRoomCount)
        {
            int lack = MinMainRoomCount - groupRoom[true].Count;
            groupRoom[true].AddRange(
                groupRoom[false].OrderBy(room => room.Size.Length())
                .Where((room, idx) => idx < lack)
                );
        }

        //select Main Rooms
        foreach(var room in groupRoom[true])
        {
            room.GetChildByType<CollisionShape2D>().DebugColor = Color.FromHtml("db56576b");
        }

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

