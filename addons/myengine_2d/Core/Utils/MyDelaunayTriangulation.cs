using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class DelaunayTriangulation
{
    /// <summary>
    /// Make Array of Vector2 to List of Triangle
    /// </summary>
    public static List<Triangle> Triangulate(Vector2I[] points)
    {
        List<Triangle> triangles = new List<Triangle>();

        foreach (Vector2 p in points)
        {
            foreach (Vector2 q in points.Where(x => x != p))
            {
                foreach (Vector2 r in points.Where(x => x != p && x != q))
                {
                    Triangle t = new Triangle(p, q, r);
                    if (IsDelaunay(t))
                    {
                        triangles.Add(t);
                    }
                }
            }
        }

        return triangles;
    }

    private static bool IsDelaunay(Triangle t)
    {
        Vector2 p = t.P;
        Vector2 q = t.Q;
        Vector2 r = t.R;

        // Check if the circumcircle of pqr is empty
        double d = (q.X - p.X) * (r.Y - p.Y) - (q.Y - p.Y) * (r.X - p.X);
        double r2 = (q.X - p.X) * (q.X - p.X) + (q.Y - p.Y) * (q.Y - p.Y);
        double r1 = (r.X - p.X) * (r.X - p.X) + (r.Y - p.Y) * (r.Y - p.Y);
        double r0 = (q.X - p.X) * (r.Y - p.Y) - (q.Y - p.Y) * (r.X - p.X);

        if (d < 0) return true; // circumcircle is empty

        // Check if the circumcircle of pqr is empty
        double r = Math.Sqrt(r2 + r1 - 2 * r0);
        if (r < 0.0001) return true; // circumcircle is empty

        return false; // circumcircle is not empty
    }
}

public class Triangle
{
    public Vector2 P { get; set; }
    public Vector2 Q { get; set; }
    public Vector2 R { get; set; }

    public Triangle(Vector2 p, Vector2 q, Vector2 r)
    {
        P = p;
        Q = q;
        R = r;
    }
}