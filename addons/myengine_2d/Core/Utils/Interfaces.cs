using Define;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// PackedScene을 static으로 소유하고 있고, 이를 이용해 Instatiate를 제공하는 New함수 제공
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IPackedSceneNode<T> where T : Node
{
    static abstract PackedScene PackedScene { get; }
    /// <summary>
    /// Instantiate T Node and return it.
    /// </summary>
    static abstract T New(Node parent = null); 
}

/// <summary>
/// PackedScene없이도 new를 통해 바로 생성 가능 해야함.
/// </summary>
interface INewableNode
{}
