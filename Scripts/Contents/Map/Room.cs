using Godot;
using System;

public partial class Room : RigidBody2D
{
    Define.RoomTypes _type;
    Vector2I _size;

    static PackedScene _scene = ResourceLoader.Load<PackedScene>("Map/room.tscn");

    [Export]
    public bool IsSelected { get; set; } = false;

    [Export]
    public Vector2I Size
    {
        get { return _size; }
        set
        {
            _size = value;
            GenerateCollisionShape();
        }
    }
    [Export]
    public Define.RoomTypes RoomType
    {
        get { return _type; }
        set
        {
            _type = value;
            GenerateRoom();
        }
    }

    public override void _PhysicsProcess(double delta)
    {

    }

    static public Room InstantiateRoom(Define.RoomTypes type, Vector2I size)
    {
        Room room = Managers.Resource.Instantiate<Room>(_scene, null);
    }

    public void GenerateRoom()
    {
        switch(RoomType)
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
        GD.Print($"Gernerate Room :  {Size.X}  * {Size.Y}");
        return;
    }

    public override void _Ready()
    {
        _scene = Managers.Resource.LoadPackedScene<Room>(Define.Scenes.Nodes, "Map/room.tscn");

        DungeonBuilder.DungeonCompleteAction -= OnDungeonCompleted;
        DungeonBuilder.DungeonCompleteAction += OnDungeonCompleted;
    }

    void GenerateCollisionShape()
    {
        var rect = new RectangleShape2D();
        rect.Size = Size;
        this.GetChildByType<CollisionShape2D>().Shape = rect;
    }

    void OnDungeonCompleted()
    {
        if (IsSelected == false)
            this.QueueFree();
        else
        {
            this.GetChildByType<CollisionShape2D>().DebugColor = Color.FromHtml("db56576b");
            this.GetChildByType<RigidBody2D>().Freeze = true;
        }

    }


}
