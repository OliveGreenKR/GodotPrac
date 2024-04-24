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

    public override void _Ready()
    {
        //Init Managers
        if(s_instance == null)
        {
            s_instance = GetNode<Managers>("/root/Managers");
        }
    }

    public override void _Process(double delta)
    {
     
    }

    public override void _ExitTree()
    {
        //clear Managers
    }
}
