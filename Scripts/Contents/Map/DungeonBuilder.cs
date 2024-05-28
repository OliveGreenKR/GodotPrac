using Godot;
using Godot.Collections;
using System;
using DelaunatorSharp;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using MEC;
using Define;
using static System.Net.Mime.MediaTypeNames;

public partial class DungeonBuilder : Node
{
    public static Action DungeonCompleteAction = () => { };

    [Export]
    public int TileSize { get; set; }
    [Export]
    public Godot.Vector2I DuegonSize { get; set; } = new Godot.Vector2I(200, 100);
    [Export]
    public int GenerateRoomCount { get; set; }
    [Export]
    public int MinMainRoomCount { get; set; }
    [Export]
    public int MaxMainRoomCount { get; set; }
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

        TileMap tilemap =  this.GetChildByType<TileMap>();


        //tilemap.SetCellsTerrainConnect(, terrainSet: 0, terrain: 0);
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
        for (int i = 0; i < GenerateRoomCount; i++)
        {
            Godot.Vector2I pos = GetRandomPointInEllipse(DuegonSize) + GetViewport().GetVisibleRect().Size.ToVector2I() / 2;
            var tmpRoom = GenerateRoomRandomlyAt(pos);
            tmpRooms.Add(tmpRoom);
            _TypeRooms[tmpRoom.RoomType].AddChild(tmpRoom, true);
        }
        //select main rooms
        float standard = new Vector2I(MinRoomSize * TileSize, MaxRoomSize * TileSize).Length() * 1f;

        List<Room> selectedRooms = SelectMainRooms(tmpRooms, standard);
        GD.Print($"selected  room count : {selectedRooms.Count}");
        
        //wait for positioning Rooms
        await this.WaitForSeconds(1f, processInPhysics: true);

        //delaunary main Rooms
        DelaunatorEx.GridPoint[] selectedPoints = selectedRooms.Select((room, i) =>
        { 
            var tmp = new DelaunatorEx.GridPoint { Vector = room.GlobalPosition.ToVector2I(), Index = i };
            tmp.Parent = tmp;
            return tmp;
        }
        ).ToArray();

        delaunator = new Delaunator(selectedPoints);

        //TODO  : Kruskal MST
        var edges = delaunator.GetEdges();
        PriorityQueue<IEdge, float> pq = new PriorityQueue<IEdge, float>();
        List<IEdge> selectedEdge = new List<IEdge>();  

        foreach (IEdge edge in edges)
        {
            pq.Enqueue(edge, edge.Length());
        }


        var dsj = new DisJointSet();
        int cnt = 0;
        var SelectEdge = (IEdge now) => 
        {
            var nowP = now.P as DelaunatorEx.GridPoint;
            var nowQ = now.Q as DelaunatorEx.GridPoint;
            if (dsj.IsUnion(nowP, nowQ) == false)
            {
                dsj.Union(nowP, nowQ);
                DrawEdges(now);
                selectedEdge.Add(now);
                cnt++;
            }
        };

        while (cnt != selectedRooms.Count - 1)
        {
            SelectEdge(pq.Dequeue());
        }
        //adding some edge 
        for( int i = 0; i < Mathf.Max(1, selectedEdge.Count() / 4) ; i++)
        {
            var now = pq.Dequeue();
            DrawEdges(now, Colors.Yellow);
            selectedEdge.Add(now);
        }

        //todo :make edge to road.

        //dugeon build finished
        DungeonCompleteAction.Invoke();
    }

    void DrawEdges( IEdge edge , Color? color = null)
    {
        Line2D drawer = new Line2D();
        drawer.Width = 2;
        
        if (color == null)
            drawer.DefaultColor = Colors.White;
        else
            drawer.DefaultColor = (Godot.Color)color;
        drawer.AddPoint(edge.P.ToVector2());
        drawer.AddPoint(edge.Q.ToVector2());
        this.AddChild(drawer);
    }

    void DrawTriangles( Delaunator delaunator)
    {
        foreach (IEdge edge in delaunator.GetEdges())
        {
            DrawEdges(edge);
            GD.Print($"{edge.Index} : drawed");
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

        

        //main room count management
        if (groupRoom[true].Count < MinMainRoomCount)
        {
            int lack = MinMainRoomCount - groupRoom[true].Count;
            groupRoom[true].AddRange(
                groupRoom[false].OrderBy(room => room.Size.Length())
                .Where((room, cnt) => cnt < lack)
                );
        } else if ( MaxMainRoomCount > MinMainRoomCount &&  groupRoom[true].Count > MaxMainRoomCount)
        {
            groupRoom[true] = groupRoom[true].GetRange(0, MaxMainRoomCount);
        }

        //Coloring Main Rooms 
        foreach (var room in groupRoom[true])
        {
            room.GetChildByType<CollisionShape2D>().DebugColor = Color.FromHtml("db56576b");
        }

        return groupRoom[true];
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

