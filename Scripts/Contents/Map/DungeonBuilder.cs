using Godot;
using Godot.Collections;
using System;
using static Godot.OpenXRInterface;
using DelaunatorSharp;
using System.Linq;

public partial class DungeonBuilder : Node
{
    public static Action DungeonCompleteAction = () => { };

    [Export]
    public int TileSize { get; set; } = 32;
    [Export]
    public Godot.Vector2I DuegonSize { get; set; } = new Godot.Vector2I(200, 100);
    [Export]
    public int RoomCount { get; set; } = 25;
    [Export]
    public int MaxRoomSize { get; set; } = 20;
    [Export]
    public int MinRoomSize { get; set; } = 5;

    public ulong? Seed { get; private set; }

    //random gernerator
    RandomNumberGenerator _rand = new RandomNumberGenerator();

    #region Tile &$ Deluaunry
    class GridPoint : IPoint
    {
        Vector2I _vector;
        public Vector2I Vector
        {
            get
            {
                return _vector;
            }
            set
            {
                _vector = value;
            }
        }
        public double X
        {
            get { return _vector.X; }
            set { _vector.X = (int)value;}
        }
        public double Y
        {
            get { return _vector.Y; }
            set { _vector.Y = (int)value; }
        }

    }

    TileMap _tileMap;
    GridPoint[] _points;

    Delaunator _delaunator;

    #endregion
    //room
    PackedScene _roomInstance;
    Dictionary<Define.RoomTypes, Node> _rooms = new Dictionary<Define.RoomTypes, Node>();

    public override void _Ready()
    {
        _rand.Seed = Seed ?? (ulong)DateTime.Now.Ticks;

        _tileMap = this.GetChildByType<TileMap>();
        _tileMap.RenderingQuadrantSize = TileSize;

        _points =  new GridPoint[0];

        _roomInstance = Managers.Resource.LoadPackedScene<Room>(Define.Scenes.Nodes,"Map/room.tscn");

        //generate Node2D each to 'Define.RoomTypes'
        Bind();
        //generating room
        for (int i =0; i < RoomCount; i++)
        {
            Godot.Vector2I pos = GetRandomPointInEllipse(DuegonSize) + GetViewport().GetVisibleRect().Size.ConvertInt()/2;
            GenerateRoom(pos);
        }
        DungeonCompleteAction.Invoke();

        //select main rooms and get point
        SelectMainRooms();
        //delaunary main Rooms



        //todp :tilemap
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
            var child = this.GetorAddChildByName<Node>(name);
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

    void SelectMainRooms()
    {

        var values = Enum.GetValues(typeof(Define.RoomTypes)).Cast<Define.RoomTypes>();

        foreach (var value in values)
        {
            Node typeNode;
            if (_rooms.TryGetValue(value, out typeNode) == false)
            {
                GD.Print($"{value} continue");
                continue;
            }
            var rooms = typeNode.GetChildrenByType<Room>();
            GD.Print($"{value} :{rooms.Count}");
            foreach (Node node in rooms)
            { 
                var room = node as Room;
                var standard = new Vector2I(MinRoomSize,MaxRoomSize).Length() * 1.1f;
                //GD.Print($"{room.Name} Size : {room.Size} / {standard}");
                if (room.Size.Length() > standard)
                {
                    _points.Append(new GridPoint() { Vector = room.Position.ConvertInt() });
                    GD.Print($"MainRoom : {room.Name}");
                    room.GetChildByType<CollisionShape2D>().DebugColor = new Color(0xdb56576b);
                }
            }
            //if (children.Count <= 0)
            //    continue;

            //var standard = MinRoomSize * MaxRoomSize * 0.95f;
            //foreach ( var childNode in children)
            //{
            //    var child = childNode as Room;

            //    if(child.Size.Length() > standard)
            //    {
            //        _points.Append(new GridPoint() { Vector = child.Position.ConvertInt() });
            //        GD.Print($"MainRoom : {child.Name}");
            //        child.GetChildByType<CollisionShape2D>().DebugColor = new Color(0xdb56576b);
            //    }
            //}
        }
    }

    #region Math
    Godot.Vector2I GetRandomPointInCircle(int radius)
    {
        return GetRandomPointInEllipse( new Godot.Vector2I(radius, radius));
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
