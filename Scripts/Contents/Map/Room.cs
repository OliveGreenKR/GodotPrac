using DelaunatorSharp;
using Godot;
using System;

public partial class Room : RigidBody2D, IPackedSceneNode<Room>
{
    Define.RoomTypes _type;
    Vector2I _size;
    bool _isSelected = false;

    static PackedScene _scene = Managers.Resource.LoadPackedScene<Room>(Define.Scenes.ContentNodes, "Map/room.tscn");
    static public PackedScene PackedScene => _scene;

    static public Room New(Node parent = null) { return Managers.Resource.Instantiate<Room>(_scene, parent); }

    public static Room New(Vector2I size, Define.RoomTypes type = Define.RoomTypes.Basic)
    {
        Room room = Room.New();
        room.Size = size;
        room.RoomType = type;

        return room;
    }

    public bool IsSelected { get; set; } = false;

    [Export]
    public Vector2I Size
    {
        get { return _size; }
        set
        {
            _size = value;
            var rect = new RectangleShape2D();
            rect.Size = value;
            this.GetChildByType<CollisionShape2D>().Shape = rect;
        }
    }
    [Export]
    public Define.RoomTypes RoomType
    {
        get { return _type; }
        set
        {
            _type = value;
            GeneratingWithRoomTypes();
            return;
        }
    }

    public override void _Ready()
    {
        DungeonCalculator.DungeonCalculationCompleteAction -= OnDungeonCompleted;
        DungeonCalculator.DungeonCalculationCompleteAction += OnDungeonCompleted;
    }

    void OnDungeonCompleted()
    {
        //try genrating room  arrording to with each types.
        if (IsSelected == false)
        {
            QueueFree();
        }
        else
        {
            this.GetChildByType<CollisionShape2D>().DebugColor = Color.FromHtml("db56576b");
            this.Freeze = true;
            GeneratingWithRoomTypes();
        }

    }

    void GeneratingWithRoomTypes()
    {
        if (IsSelected == false) return;

        switch (_type)
        {
            case Define.RoomTypes.Basic:
                {
                    GD.Print("Basic Romm Genrated");
                }
                break;
            case Define.RoomTypes.A:
                {
                    GD.Print("A Romm Genrated");
                }
                break;
            case Define.RoomTypes.B:
                {
                    GD.Print("B Romm Genrated");
                }
                break;
        }
    }

    /// <param name="direction"> relative  to Room's position </param>
    public void GenerateRandomDoor(Vector2 direction)
    {

    }


}
