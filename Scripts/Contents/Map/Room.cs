using Godot;
using System;

public partial class Room : RigidBody2D
{
    public Action testAction;

    Define.RoomTypes _type;
    Vector2I _size;

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
        if (testAction != null)
            testAction.Invoke();
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
        DungeonBuilder.DungeonCompleteAction -= OnDungeonCompleted;
        DungeonBuilder.DungeonCompleteAction += OnDungeonCompleted;
    }

    void GenerateCollisionShape()
    {
        var rect = new RectangleShape2D();
        rect.Size = Size;
        this.GetChildByType<CollisionShape2D>().Shape = rect;
    }

    #region Callback
    void OnDungeonCompleted()
    {
        if (this.IsValid() == false)
            return;
        //todo : collision deacitvated or queue free

    }
    #endregion

}
