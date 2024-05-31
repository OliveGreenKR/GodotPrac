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


    public static Vector2 GetRandomPointInCircle(float radius)
    {
        return GetRandomPointInEllipse(new Vector2(radius, radius));
    }

    public static Vector2 GetRandomPointInEllipse(Vector2 size)
    {
        float theta = 2 * Mathf.Pi * Managers.Random.Randf();
        float u = Managers.Random.Randf() + Managers.Random.Randf();
        float r = u > 1 ? 2 - u : u;

        float x = size.X * r * Mathf.Cos(theta);
        float y = size.Y * r * Mathf.Sin(theta);
        return new Vector2(x, y);
    }

    /// <summary>
    ///  get Point on the Line with slope theta passing through Center
    /// </summary>
    /// <param name="theta"> relative to x axis</param>
    /// <returns></returns>
    public static Vector2 GetPointOnLine(Vector2 center, float dx, float theta)
    {
        double y = Mathf.Tan(theta) * (dx) + center.Y;
        double x = center.X + dx;
        return new Vector2((float)x, (float)y);
    }

}