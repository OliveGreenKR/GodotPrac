using DelaunatorSharp;
using Godot;
using System;

public partial class Room : RigidBody2D, IPackedSceneNode<Room>, INewableNode
{
    public Action RoomArrangementCompleteAction = () => { };

    Define.RoomTypes _type;
    Vector2I _size;
    bool _isSelected = false;

    Vector2 _position;

    static PackedScene _scene = Managers.Resource.LoadPackedScene<Room>(Define.Scenes.ContentNodes, "Map/room.tscn");
    static public PackedScene PackedScene {get{return _scene;}}

    static public Room New(Node parent = null) 
    { 
        return Managers.Resource.Instantiate<Room>(_scene, parent); 
    }

    public static Room New(Vector2I size, Define.RoomTypes type = Define.RoomTypes.Basic)
    {
        Room room = Room.New();
        room.Size = size;
        room.RoomType = type;

        return room;
    }

    public Room()
    {
        //rigid body setting
        CanSleep = true;
        LockRotation = true;
        GravityScale = 0;
       
        CollisionLayer = (int)Define.Physics2D.DungeonRoom;
        CollisionMask = (int)Define.Physics2D.DungeonRoom; 
        //collisionshape
        var collision = this.GetOrAddChildByType<CollisionShape2D>();
        collision.Shape = new RectangleShape2D();

    }

    public bool IsSelected { get; set; } = false;

    [Export]
    public Vector2I Size
    {
        get { return _size; }
        set
        {
            _size = value;
            var shape = this.GetOrAddChildByType<CollisionShape2D>().Shape as RectangleShape2D;
            shape.Size = value;
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

    public override void _PhysicsProcess(double delta)
    {
        if( _position != GlobalPosition )
            _position = GlobalPosition;
        else
        {
            RoomArrangementCompleteAction?.Invoke();
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
    public void GenerateRandomDoor(Vector2 position)
    {
        Door door = Door.New(this);
        door.GlobalPosition = position;
    }

    
}
