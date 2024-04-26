using Godot;
using System;
using static Godot.OpenXRInterface;


public partial class DungeonBuilder : Node
{
    public static Action DungeonCompleteAction = () => { };

    [Export]
    public int TileSize { get; set; } = 32;
    [Export]
    public Vector2 DuegonSize { get; set; } = new Vector2(200, 100);
    [Export]
    public int MaxRoomSize { get; set; } = 30;
    [Export]
    public int MinRoomSize { get; set; } = 5;

    public ulong? Seed { get; private set; }

    RandomNumberGenerator _rand;
    TileMap _tileMap;
    PackedScene _room;

    public override void _Ready()
    {
        _rand = new RandomNumberGenerator(); 
        _rand.Seed = Seed ?? (ulong)DateTime.Now.Ticks;

        _tileMap = this.GetChildByType<TileMap>();
        _tileMap.RenderingQuadrantSize = TileSize;

        _room = Managers.Resource.LoadScene<Room>(Define.Scenes.Scenes,"Map/room.tscn");

        //_tileMap.Set

        //test
        for(int i =0; i < 35; i++)
        {
            Vector2 pos = GetRandomPointInEllipse(DuegonSize) + GetViewport().GetVisibleRect().Size / 2;
            GenerateRoom(pos);
        }
        DungeonCompleteAction.Invoke();

        //var timer = new Timer();
        //AddChild(timer);
        //timer.Timeout += () =>
        //{
        //    Vector2 pos = GetRandomPointInEllipse(DuegonSize) + GetViewport().GetVisibleRect().Size / 2;
        //    GenerateRoom(pos);
        //};
        //timer.Start(0.3f);
    }

    public override void _Process(double delta)
    {
    }

    //generate Room
    void GenerateRoom(Vector2 position)
    {
        //init Room  at point with random size
        Room room = Managers.Resource.Instantiate<Room>(_room, null);
        room.Size = new Vector2I(_rand.RandiRange(MinRoomSize, MaxRoomSize),
                                 _rand.RandiRange(MinRoomSize, MaxRoomSize));
        AddChild(room);
        room.Position = position;
    }

    #region Math
    Vector2 GetRandomPointInCircle(float radius)
    {
        return GetRandomPointInEllipse( new Vector2(radius,radius));
    }

    Vector2 GetRandomPointInEllipse(Vector2 size)
    {
        float theta = 2 * Mathf.Pi * _rand.Randf();
        float u = _rand.Randf() + _rand.Randf();
        float r = u > 1 ? 2 - u : u;

        int x = RoundTileSize(size.X * r * Mathf.Cos(theta), TileSize);
        int y = RoundTileSize(size.Y * r * Mathf.Sin(theta), TileSize);
        return new Vector2(x, y);
    }

    /// <summary>
    /// round N to Multiples of M
    /// </summary>
    int RoundTileSize(float n, int tileSize)
    {
        return Mathf.FloorToInt(((n + tileSize - 1) / tileSize) * tileSize);
    }
    #endregion

}
