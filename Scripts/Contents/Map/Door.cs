using Godot;
using System;
using System.Security.Cryptography;

public partial class Door : Node2D, IPackedSceneNode<Door>
{

    static PackedScene _scene = Managers.Resource.LoadPackedScene<Door>(Define.Scenes.ContentNodes, "Map/room.tscn");
    public static PackedScene PackedScene => _scene;
    public static Door GetNewInstance(Node parent = null ) { return Managers.Resource.Instantiate<Door>(_scene, parent); }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		var collision = this.GetChildByType<CollisionSprite2D>();
		collision.Size = Managers.Tile.TileSizeVector;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    
}
