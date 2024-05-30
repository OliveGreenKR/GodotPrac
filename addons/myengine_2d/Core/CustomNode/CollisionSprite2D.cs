using Godot;
using System;

public partial class CollisionSprite2D : CollisionShape2D, IGeneratableScene
{
	Vector2 _size = new Vector2();

	Sprite2D _sprite = null;

	static PackedScene _scene = Managers.Resource.LoadPackedScene<CollisionSprite2D>(Define.Scenes.CoreNodes);

	[Export]
	public Vector2 Size { get { return _size; } 
		set 
		{
			_size = value;

			if (_sprite == null || _sprite.Texture == null) return;

			var rect = new RectangleShape2D();
			rect.Size = value;

			this.Shape = rect;
            var ratio = _size / _sprite.Texture.GetSize();
			_sprite.Scale = ratio;

        } }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_sprite =  this.GetOrAddChildByType<Sprite2D>();
	}
}