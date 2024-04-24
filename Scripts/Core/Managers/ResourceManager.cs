using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ResourceManager
{
    public T Load<T>(string path, ResourceLoader.CacheMode mode =  ResourceLoader.CacheMode.Reuse) where T : GodotObject
    {
        return ResourceLoader.Load<T>(path, "", mode);
    }
    /// <summary>
    /// Instantiate Node and Adding Child to parent
    /// </summary>
    public Node Instantiate(string path, Node parent , Node.InternalMode mode = Node.InternalMode.Disabled)
    {
        PackedScene scene =  Load<PackedScene>(path);
        var node = scene.Instantiate(); 
        if( parent != null)
            parent.AddChild(node, @internal : mode);
        return node;
    }
    /// <summary>
    /// Instantiate Node and Adding Child to parent
    /// </summary>
    public Node Instantiate(PackedScene scene, Node parent, Node.InternalMode mode = Node.InternalMode.Disabled)
    {
        var node = scene.Instantiate();
        if (parent != null)
            parent.AddChild(node, @internal: mode);
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

