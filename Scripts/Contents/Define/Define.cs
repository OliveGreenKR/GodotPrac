using DelaunatorSharp;
using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Define;

public enum Scenes
{
    Nodes,
    GameScenes,
}

public enum RoomTypes
{
    Basic = 0,
    A,
    B,
}

public class GridPoint : IPoint
{
    Vector2I _vec;

    public Vector2I Vector { get => _vec; set => _vec = value; }
    public int Index { get; set; }

    public double X { get => _vec.X; set => _vec.X = (int)value; }
    public double Y { get => _vec.Y; set => _vec.Y = (int)value; }

    static public implicit operator GridPoint(Vector2I vec) { return new GridPoint { Vector = (Vector2I)vec }; }
    static public implicit operator Vector2I(GridPoint gp) { return new Vector2I { X = (int)gp.X, Y = (int)gp.Y }; }

}

