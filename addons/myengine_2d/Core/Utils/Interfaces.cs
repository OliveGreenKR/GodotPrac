using Define;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IPackedSceneNode<T> where T : Node
{
    static abstract PackedScene PackedScene { get; }
    static abstract T GetNewInstance(Node parent = null); 
}
