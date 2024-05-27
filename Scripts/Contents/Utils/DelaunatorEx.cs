using System.Runtime.CompilerServices;
using DelaunatorSharp;
using Godot;
public static class DelaunatorEx
{
    public static Vector2 ToVector2 (this IPoint point)
    {
        return new Vector2 { X = (float)point.X, Y = (float)point.Y };
    }

    public static float Length( this IEdge edge)
    {
        return (edge.Q.ToVector2() - edge.P.ToVector2()).Length();
    }

    public class GridPoint : IPoint, IDisJointable
    {
        Vector2 _vec;

        public Vector2 Vector { get => _vec; set => _vec = value; }
        public int Index { get; set; }

        public double X { get => _vec.X; set => _vec.X = (int)value; }
        public double Y { get => _vec.Y; set => _vec.Y = (int)value; }

        public IDisJointable Parent { get; set; } 

        static public implicit operator GridPoint(Vector2I vec) { return new GridPoint { Vector = vec }; }
        static public implicit operator Vector2(GridPoint gp) { return new Vector2 { X = (float)gp.X, Y = (float)gp.Y }; }
    }

}