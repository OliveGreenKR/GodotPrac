using Godot;
using System;

public partial class Managers : Node
{
    static Managers s_instance;
    static Managers Instance { get { return s_instance; } }

    #region Contents
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
        s_instance = this;
    }

    public override void _Process(double delta)
    {
     
    }

    public override void _ExitTree()
    {
        //clear Managers
    }
}
