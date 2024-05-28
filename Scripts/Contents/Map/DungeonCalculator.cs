using Godot;
using Godot.Collections;
using System;
using DelaunatorSharp;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

public partial class DungeonCalculator : Node
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
    Godot.Collections.Dictionary<Define.RoomTypes, Node> _typeRooms = new Godot.Collections.Dictionary<Define.RoomTypes, Node>();

    public override async void _Ready()
    {
        Bind();

        await Task.Run(() => { GenerateDungeon(); });
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
            _typeRooms.Add((Define.RoomTypes)i, child);
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
            Godot.Vector2I pos = GetRandomPointInEllipse(DungeonSize) + GetViewport().GetVisibleRect().Size.ToVector2I() / 2;
            var tmpRoom = GenerateRoomRandomSizedAt(pos);
            tmpRooms.Add(tmpRoom);
            _typeRooms[tmpRoom.RoomType].AddChild(tmpRoom, true);
        }

        //wait for positioning Rooms
        await this.WaitForSeconds(1f, processInPhysics: true);

        //select main rooms
        float standard = new Vector2I(MinRoomSize * Managers.Tile.TileSize, MaxRoomSize * Managers.Tile.TileSize).Length() * 1f;

        List<Room> selectedRooms = SelectMainRooms(tmpRooms, standard);
        GD.Print($"selected  room count : {selectedRooms.Count}");

        //delaunary main Rooms
        DelaunatorEx.GridPoint[] selectedPoints = selectedRooms.Select((room, i) =>
        { 
            var tmp = new DelaunatorEx.GridPoint { Vector = room.GlobalPosition.ToVector2I(), Index = i };
            tmp.Parent = tmp;
            return tmp;
        }
        ).ToArray();

        delaunator = new Delaunator(selectedPoints);
        var selectedEdge = delaunator.MakeMstKruskal(addSomeExtra: true);

        foreach (IEdge edge in selectedEdge)
        {
            DrawEdges(edge, Colors.Green);
        }
        //todo :make edge to road.
        DungeonCalculationCompleteAction.Invoke();
    }

    #region debug-visualization
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
    #endregion

    //generate Single Room
    Room GenerateRoomRandomSizedAt(Godot.Vector2 position)
    {
        var size = new Vector2I(Managers.Random.RandiRange(MinRoomSize * Managers.Tile.TileSize, MaxRoomSize * Managers.Tile.TileSize),
                                 Managers.Random.RandiRange(MinRoomSize * Managers.Tile.TileSize, MaxRoomSize * Managers.Tile.TileSize));
        Room room = Room.New(size);
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

        foreach (var room in groupRoom[true])
        {
            room.IsSelected = true;
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
        float theta = 2 * Mathf.Pi * Managers.Random.Randf();
        float u = Managers.Random.Randf() + Managers.Random.Randf();
        float r = u > 1 ? 2 - u : u;

        int x = Utils.RoundTileSize(size.X * r * Mathf.Cos(theta));
        int y = Utils.RoundTileSize(size.Y * r * Mathf.Sin(theta));
        return new Godot.Vector2I(x, y);
    }


    #endregion

}

