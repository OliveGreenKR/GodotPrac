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
    static abstract T New(Node parent = null); 
}

/// <summary>
/// PackedScene없이도 new를 통해 바로 생성가능함. _Ready에 미리 사전 작업을 완료함.
/// </summary>
public interface INewableNode
{
    void InitNode();
}
