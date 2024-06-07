using Godot;
using Godot.Collections;
using System;
using DelaunatorSharp;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public partial class DungeonCalculator : Node2D
{
    public static Action DungeonCalculationCompleteAction = () => { };

    [Export]
    public Godot.Vector2I DungeonSize { get; set; } = new Godot.Vector2I(200, 100);
    [Export]
    public int GenerateRoomCount { get; set; } = 35;
    [Export]
    public int MinMainRoomCount { get; set; } = 6;
    [Export]
    public int MaxMainRoomCount { get; set; } = 10;
    [Export]
    public int MaxRoomSize { get; set; } = 10;
    [Export]
    public int MinRoomSize { get; set; } = 4;

    //room
    //Godot.Collections.Dictionary<Define.RoomTypes, Node> _typeRooms = new Godot.Collections.Dictionary<Define.RoomTypes, Node>();
    List<Room> _rooms = new List<Room>();
    List<Vector2I> _roads = new List<Vector2I>(); 

    int _arrangedRoomCount = 0;

    public override void _Ready()
    {
        //Bind();
        GenerateRooms();
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
            //_typeRooms.Add((Define.RoomTypes)i, child);
        }
    }

    public void GenerateRooms()
    {
        //generating room
        for (int i = 0; i < GenerateRoomCount; i++)
        {
            Godot.Vector2 pos = Utils.GetRandomPointInEllipse(DungeonSize);
            var room = GenerateRoomRandomSizedAt(pos);
            _rooms.Add(room);
            room.RoomArrangementCompleteAction += OnRoomArragementCompleted;
        }
    }

    void CalculatingGraph()
    {
        Delaunator delaunator;

        //select main rooms
        float standard = new Vector2I(MinRoomSize * Managers.Tile.TileSize, MaxRoomSize * Managers.Tile.TileSize).Length() * 1f;

        List<Room> selectedRooms = SelectMainRooms(_rooms, standard);
        GD.Print($"selection Room Count : {selectedRooms.Count}");

        //delaunary main Rooms
        DelaunatorEx.GridPoint[] selectedPoints = selectedRooms.Select((room, i) =>
        {
            var tmp = new DelaunatorEx.GridPoint { Vector = room.GlobalPosition.ToVector2I(), Index = i };
            tmp.Parent = tmp; //for Kruskal, make DisjointSet
            return tmp;
        }
        ).ToArray();

        delaunator = new Delaunator(selectedPoints);
        var selectedEdge = delaunator.MakeMstKruskal(addSomeExtra: true);

        //test :make edge to road.

        foreach (var room in selectedRooms)
        {
            TilingRoom(room);
        }

        foreach (IEdge edge in selectedEdge)
        {
            DrawEdges(edge, Colors.Green);
            //TilingEdge(edge);
        }

        //Invoke
        DungeonCalculationCompleteAction.Invoke();
    }
    void OnRoomArragementCompleted()
    {
        if (++_arrangedRoomCount == GenerateRoomCount)
        {
            CalculatingGraph();
        }
    }

    #region Tiling

    void TilingRoom(Room room)
    {
        var TM = Managers.Tile.DungeonTM;

        var topleft = (room.GlobalPosition - room.Size / 2).ToVector2I();
        var bottomright = (room.GlobalPosition + room.Size / 2).ToVector2I();

        Array<Vector2I> roomCells = new Array<Vector2I>();
        Array<Vector2I> wallCells = new Array<Vector2I>();

        var mapTop = TM.LocalToMap(topleft);
        var mapBottom = TM.LocalToMap(bottomright);
        for (int x = topleft.X; x < bottomright.X; x += Managers.Tile.TileSize)
        {
            for (int y = topleft.X; y < bottomright.X; y += Managers.Tile.TileSize)
            {
                var mapCoord = TM.LocalToMap(ToLocal(new Vector2I(x, y)));

                if (mapTop.Or(mapCoord) || mapBottom.Or(mapCoord))
                {
                    //wall
                    wallCells.Add(mapCoord);
                }
                else
                {
                    //ground
                    roomCells.Add(mapCoord);
                }

            }
        }

        TM.SetCellsTerrainConnect(1, roomCells, 0, 0);
        TM.SetCellsTerrainConnect(2, wallCells, 0, 2);
    }

    void TilingEdge(IEdge edge)
    {
        var tilesize = Managers.Tile.TileSize;

        var TM = Managers.Tile.DungeonTM;
        Array<Vector2I> cells = new Array<Vector2I>();

        var p = edge.P.ToVector2();
        var q = edge.Q.ToVector2();

        var delta = q - p;

        if( delta.X > 0)
        {
            for (int i = 0; i < delta.X; i+= tilesize)
            {
                //right
                p += new Vector2(i, 0);
                cells.Add(TM.LocalToMap(ToLocal(p)));
            }

        }
        else if ( delta.X < 0)
        {
            for (int i = 0; i < -delta.X; i-= tilesize )
            {
                //left
                p += new Vector2(i, 0);
                cells.Add(TM.LocalToMap(ToLocal(p)));
            }
        }

        if (delta.Y > 0)
        {

            for (int i = 0; i < delta.Y; i += tilesize)
            {
                //up
                p += new Vector2(0, i);
                cells.Add(TM.LocalToMap(ToLocal(p)));
            }
        }
        else if( delta.Y < 0)
        {
            for (int i = 0; i < -delta.Y; i -= tilesize)
            {
                //left
                p +=  new Vector2(0, i);
                cells.Add(TM.LocalToMap(ToLocal(p)));
            }
        }
        //road to Layer:Wall(2)
        TM.SetCellsTerrainConnect(2,cells,0, 1);
    }
    #endregion

    #region debug-visualization
    void DrawEdges( IEdge edge , Color? color = null)
    {
        var from = edge.P.ToVector2();
        var to = edge.Q.ToVector2();
        Managers.Debug.DrawLine2D(from, to, color : color);
    }

    void DrawTriangles( Delaunator delaunator)
    {
        foreach (IEdge edge in delaunator.GetEdges())
        {
            DrawEdges(edge);
        }
    }
    #endregion

    //generate Single Room
    Room GenerateRoomRandomSizedAt(Godot.Vector2 position)
    {
        var size = new Vector2I(Managers.Random.RandiRange(MinRoomSize * Managers.Tile.TileSize, MaxRoomSize * Managers.Tile.TileSize),
                                 Managers.Random.RandiRange(MinRoomSize * Managers.Tile.TileSize, MaxRoomSize * Managers.Tile.TileSize));
        Room room = new Room();
        room.GlobalPosition = position;
        room.Size = size;
        this.AddChild(room,true);
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

        foreach (var room in groupRoom[true])
        {
            room.IsSelected = true;
        }

        return groupRoom[true];
    }

}

