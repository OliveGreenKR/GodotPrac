using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ResourceManager
{
    /// <summary>
    /// Load C# class to PackedScnene  via class Name.
    /// </summary>
    public PackedScene LoadPackedScene<T>(Define.Scenes type,string path =null, ResourceLoader.CacheMode mode = ResourceLoader.CacheMode.Reuse) where T : GodotObject
    {
        if(path == null)
        {
            path = typeof(T).Name.ToSnakeCase();
        }

        string absPath = "";
        if (path.LastIndexOf(".tscn") < 0)
            path += ".tscn";

        switch (type)
        {
            case Define.Scenes.CoreNodes:
                {
                    absPath = "res://addons/myengine_2d/Core/CustomNode/" + path;
                }
                break;
            case Define.Scenes.ContentNodes:
                {
                    absPath = "res://Scenes/" + path;
                }
                break;
            case Define.Scenes.GameScenes:
                {
                    absPath = "res://Scenes/GameScene/" + path;
                }
                break;
        }
        return Load<PackedScene>(absPath, mode);
    }

    /// <summary>
    /// Godot ResourceLoader.Load Wrapper
    /// </summary>
    public T Load<T>(string path, ResourceLoader.CacheMode mode =  ResourceLoader.CacheMode.Reuse) where T : GodotObject
    {
        return ResourceLoader.Load<T>(path, "", mode);
    }

    /// <summary>
    /// Instantiate Node and Adding Child to parent
    /// </summary>
    public T Instantiate<T>(string path, Node parent, Node.InternalMode mode = Node.InternalMode.Disabled) where T : Node
    {
        return Instantiate(path, parent, mode) as T;
    }

    /// <summary>
    /// Instantiate Node and Adding Child to parent
    /// </summary>
    public Node Instantiate(string path, Node parent , Node.InternalMode mode = Node.InternalMode.Disabled)
    {
        PackedScene scene =  Load<PackedScene>(path);
        var node = scene.Instantiate(); 
        if( parent != null)
            parent.AddChildDeferred(node, @internal : mode);
        return node;
    }

    /// <summary>
    /// Instantiate Scene to Node and Adding Child to parent
    /// </summary>
    public T Instantiate<T>(PackedScene scene, Node parent, Node.InternalMode mode = Node.InternalMode.Disabled) where T : Node
    {
        return Instantiate(scene, parent, mode) as T;
    }

    /// <summary>
    /// Instantiate Scene to Node and Adding Child to parent
    /// </summary>
    public Node Instantiate(PackedScene scene, Node parent, Node.InternalMode mode = Node.InternalMode.Disabled)
    {
        var node = scene.Instantiate();
        if (parent != null)
            parent.AddChildDeferred(node, @internal: mode);
        return node;
    }

    public void Destroy(GodotObject obj)
    {
        if(obj is Node objt)
        {
            objt.QueueFree();
            return;
        }
        obj.Free();
    }

}

