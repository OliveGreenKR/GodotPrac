using Godot;
using System;

public partial class Room : RigidBody2D, IGeneratableScene
{
    Define.RoomTypes _type;
    Vector2I _size;
    bool _isSelected = false;

    static PackedScene _scene = Managers.Resource.LoadPackedScene<Room>(Define.Scenes.Nodes, "Map/room.tscn");
    public static Room New(Vector2I size, Define.RoomTypes type = Define.RoomTypes.Basic)
    {
        Room room = Managers.Resource.Instantiate<Room>(_scene, null);
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

            switch (value)
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
            return;
        }
    }

    public override void _PhysicsProcess(double delta)
    {

    }

    public override void _Ready()
    {
        DungeonCalculator.DungeonCalculationCompleteAction -= OnDungeonCompleted;
        DungeonCalculator.DungeonCalculationCompleteAction += OnDungeonCompleted;
    }

    void OnDungeonCompleted()
    {
        if (IsSelected == false)
        {
            QueueFree();
        }
        else
        {
            this.GetChildByType<CollisionShape2D>().DebugColor = Color.FromHtml("db56576b");
            this.Freeze = true;
        }
    }

    public void GenerateRandomDoor(Vector2 direction)
    {
        var dir = direction.GetDominantFactor();

    }

  
}
