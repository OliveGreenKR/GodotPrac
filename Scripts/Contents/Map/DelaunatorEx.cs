using System.Runtime.CompilerServices;
using DelaunatorSharp;
using Godot;
public static class DelaunatorEx
{
    public static Vector2 ToVector2 (this IPoint point)
    {
        return new Vector2 { X = (float)point.X, Y = (float)point.Y };
    }

    public static float Length( IEdge edge)
    {
        return (edge.Q.ToVector2() - edge.P.ToVector2()).Length();
    }
}