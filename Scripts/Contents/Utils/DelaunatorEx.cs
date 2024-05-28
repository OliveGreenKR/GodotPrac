using System.Collections.Generic;
using System.Linq;
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

    /// <summary>
    /// Making a MST based on Edge Length.and return selected Edge
    /// </summary>
    public static List<IEdge> MakeMstKruskal(this Delaunator delaunator, bool addSomeExtra = false, int nodeCount = -1)
    {
        var edges = delaunator.GetEdges();
        PriorityQueue<IEdge, float> pq = new PriorityQueue<IEdge, float>();
        List<IEdge> selectedEdge = new List<IEdge>();

        foreach (IEdge edge in edges)
        {
            pq.Enqueue(edge, edge.Length());
        }

        var dsj = new DisJointSet();
        int cnt = 0;
        var SelectEdge = (IEdge now) =>
        {
            var nowP = now.P as DelaunatorEx.GridPoint;
            var nowQ = now.Q as DelaunatorEx.GridPoint;
            if (dsj.IsUnion(nowP, nowQ) == false)
            {
                dsj.Union(nowP, nowQ);
                selectedEdge.Add(now);
                cnt++;
            }
        };

        if (nodeCount < 0)
            nodeCount = delaunator.Points.Count();

        while (cnt != nodeCount -1)
        {
            SelectEdge(pq.Dequeue());
        }
        //adding some edge 
        if (addSomeExtra)
        {
            for (int i = 0; i < Mathf.Max(1, selectedEdge.Count() / 4); i++)
            {
                var now = pq.Dequeue();
                selectedEdge.Add(now);
            }
        }

        return selectedEdge;
    }

}