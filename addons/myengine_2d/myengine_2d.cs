#if TOOLS
using Godot;
using System;

[Tool]
public partial class myengine_2d : EditorPlugin
{
    public override void _EnterTree()
    {
        // Initialization of the plugin goes here.
        //res://addons/myengine_2d/Core/CustomNode/ContinuosArea2D.cs
        var script = GD.Load<Script>("res://addons/myengine_2d/Core/CustomNode/ContinuosArea2D.cs");
        //res://addons/myengine_2d/Icon/ContinuousArea2D.svg
        var texture = GD.Load<Texture2D>("res://addons/myengine_2d/Icon/ContinuousArea2D.svg");
        AddCustomType("ContinuosArea2D", "Area2D", script, texture);
    }

    public override void _ExitTree()
    {
        // Clean-up of the plugin goes here.
        // Always remember to remove it from the engine when deactivated.
        RemoveCustomType("ContinuosArea2D");
    }
}
#endif
