using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class DelaunayTriangulation
{
    public static List<Triangle> Triangulate(List<Point> points)
    {
        List<Triangle> triangles = new List<Triangle>();

        // Create a sweep event queue
        List<Vertex> vertecies = new List<Vertex>();

        // Add all points to the event queue
        foreach (Point p in points)
        {
            vertecies.Add(new Vertex(p, 0));
        }

        // Sort the event queue by y-coordinate
        vertecies.Sort((a, b) => a.Point.Y.CompareTo(b.Point.Y));

        // Process the event queue
        while (vertecies.Count > 0)
        {
            Vertex e = vertecies.First();
            vertecies.RemoveAt(0);

            // Find the point with the smallest y-coordinate
            Point p = e.Point;
            Point q = null;
            Point r = null;

            // Find the points that are to the right of p
            foreach (Vertex ne in vertecies)
            {
                if (ne.Point.Y == p.Y)
                {
                    if (ne.Point.X > p.X)
                    {
                        q = ne.Point;
                    }
                    else
                    {
                        r = ne.Point;
                    }
                }
            }

            // Create a new triangle
            Triangle t = new Triangle(p, q, r);
            triangles.Add(t);

            // Add new events to the queue
            if (q != null)
            {
                vertecies.Add(new Vertex(q, 1));
            }
            if (r != null)
            {
                vertecies.Add(new Vertex(r, 1));
            }
        }

        return triangles;
    }
}

public class Point
{
    public double X { get; set; }
    public double Y { get; set; }

    public Point(double x, double y)
    {
        X = x;
        Y = y;
    }
}

public class Triangle
{
    public Point P { get; set; }
    public Point Q { get; set; }
    public Point R { get; set; }

    public Triangle(Point p, Point q, Point r)
    {
        P = p;
        Q = q;
        R = r;
    }
}

public class Vertex
{
    public Point Point { get; set; }
    public int Type { get; set; }

    public Vertex(Point p, int type)
    {
        Point = p;
        Type = type;
    }
}

/* »ç¿ë¹ý
 * 
 * 
List<Point> points = new List<Point>();
points.Add(new Point(0, 0));
points.Add(new Point(1, 0));
points.Add(new Point(1, 1));
points.Add(new Point(0, 1));

List<Triangle> triangles = DelaunayTriangulation.Triangulate(points);

foreach (Triangle t in triangles)
{
    Console.WriteLine($"Triangle: ({t.P.X}, {t.P.Y}) - ({t.Q.X}, {t.Q.Y}) - ({t.R.X}, {t.R.Y})");
}

 */