using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

//public class DelaunayTriangulation 
//{
//    Vector2[] Points { get; set; }
//    Triangle[] Triangles { get; set; }  


//    DelaunayTriangulation(Vector2[] points) { Points = points; }

//    public static List<Triangle> Triangulate(List<Vector2> points)
//    {
//        List<Triangle> triangles = new List<Triangle>();

//        // Create a sweep event queue
//        List<Vector2> vertecies = new List<Vector2>();

//        // Add all points to the event queue
//        foreach (Vector2 p in points)
//        {
//            vertecies.Add(p);
//        }

//        // Sort the event queue by y-coordinate
//        vertecies.Sort((a, b) => a.Y.CompareTo(b.Y));

//        // Process the event queue
//        while (vertecies.Count > 0)
//        {
//            Vector2 e = vertecies.First();
//            vertecies.RemoveAt(0);

//            // Find the Vector2 with the smallest y-coordinate
//            Vector2 p = e;
//            Vector2 q;
//            Vector2 r;

//            // Find the points that are to the right of p
//            foreach (Vector2 ne in vertecies)
//            {
//                if (ne.Y == p.Y)
//                {
//                    if (ne.X > p.X)
//                    {
//                        q = ne;
//                    }
//                    else
//                    {
//                        r = ne;
//                    }
//                }
//            }

//            // Create a new triangle
//            Triangle t = new Triangle(p, q, r);
//            triangles.Add(t);

//            // Add new events to the queue
//            if (q != null)
//            {
//                vertecies.Add(new Vector2(q, 1));
//            }
//            if (r != null)
//            {
//                vertecies.Add(new Vector2(r, 1));
//            }
//        }

//        return triangles;
//    }
//}

//public class Triangle
//{
//    public Vector2 P { get; set; }
//    public Vector2? Q { get; set; }
//    public Vector2? R { get; set; }

//    public Triangle(Vector2 p, Vector2? q, Vector2? r)
//    {
//        P = p;
//        Q = q;
//        R = r;
//    }
//}

//public class Vector2
//{
//    public Vector2 Vector2 { get; set; }
//    public Define.RoomTypes Type { get; set; }

//    public Vector2(Vector2 p, Define.RoomTypes type = Define.RoomTypes.Basic)
//    {
//        Vector2 = p;
//        Type = type;
//    }
//}

/* »ç¿ë¹ý
 * 
 * 
List<Vector2> points = new List<Vector2>();
points.Add(new Vector2(0, 0));
points.Add(new Vector2(1, 0));
points.Add(new Vector2(1, 1));
points.Add(new Vector2(0, 1));

List<Triangle> triangles = DelaunayTriangulation.Triangulate(points);

foreach (Triangle t in triangles)
{
    Console.WriteLine($"Triangle: ({t.P.X}, {t.P.Y}) - ({t.Q.X}, {t.Q.Y}) - ({t.R.X}, {t.R.Y})");
}

 */