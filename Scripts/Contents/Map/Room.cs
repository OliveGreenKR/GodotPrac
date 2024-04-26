using Godot;
using System;

public partial class Room : Node2D
{
    //divergent room type
    public enum RoomTypes 
    {
        Basic,
        A,
        B,
    }

    RoomTypes _type;
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
    public RoomTypes RoomType
    {
        get { return _type; }
        set
        {
            _type = value;
            GenerateRoom();
        }
    }

    RigidBody2D _body;

    public void GenerateRoom()
    {
        switch(RoomType)
        {
            case RoomTypes.Basic:
                {
                    GD.Print("Basic Romm Genrated");
                }
                break;
            case RoomTypes.A:
                {
                    GD.Print("A Romm Genrated");
                }
                break;
            case RoomTypes.B:
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
        if (_body.IsValid() == false)
            return;
        _body.Sleeping = true;
    }
    #endregion

}
