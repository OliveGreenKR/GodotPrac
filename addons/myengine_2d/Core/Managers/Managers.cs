using Godot;
using System;

public partial class Managers : Node
{
    static Managers s_instance;
    static Managers Instance { get { return s_instance; } }

    #region Contents
    TileManager _tile = new TileManager();
    public static TileManager Tile {  get {  return s_instance._tile; } } 

    RandomNumberGenerator _rand = new RandomNumberGenerator();
    public static RandomNumberGenerator Random { get { return s_instance._rand; } } 

    #endregion

    #region Core
    ResourceManager _resource = new ResourceManager();
    public static ResourceManager Resource {  get { return s_instance._resource; } }
    #endregion

    public override void _EnterTree()
    {
        if (s_instance != null)
        {
            this.QueueFree(); // The Singletone is already loaded, kill this instance
        }
        else
        {
            s_instance = this;
            Init();
        }
    }

    void Init()
    {
        _rand.Seed = (ulong)DateTime.Now.Ticks;
    }

    public override void _Process(double delta)
    {
     
    }

    public override void _ExitTree()
    {
        //clear Managers
    }
}
