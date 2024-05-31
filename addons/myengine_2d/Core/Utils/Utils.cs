using DelaunatorSharp;
using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;

public class Utils
{
    public static T GetChildByType<T>(Node parent, string name=null, bool recursive = false) where T : Node
    {
        if(!parent.IsValid())
            return null;
        return parent.GetChildByType<T>(recursive);
    }

    public static int RoundTileSize(float n)
    {
        return RoundIntSize(n, Managers.Tile.TileSize);
    }

    public static int RoundIntSize(float n, int size)
    {
        return Mathf.FloorToInt(((n + size - 1) / size) * size);
    }

    /// <summary>
    ///  get Point on the Line with slope theta passing through Center
    /// </summary>
    /// <param name="theta"> relative to x axis</param>
    /// <returns></returns>
    public Vector2 GetPointOnLine(Vector2 center, float dx , float theta)
    {
        double y = Mathf.Tan(theta) * (dx) + center.Y;
        double x = center.X + dx;
        return new Vector2((float)x, (float)y); 
    }

    

}