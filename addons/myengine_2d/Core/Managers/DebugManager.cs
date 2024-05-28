using DelaunatorSharp;
using Godot;
using System;

public class DebugManager
{
    Node _node = new Node();

    Color _defaultColor = Colors.WhiteSmoke;

    public Node Node { get { return _node; } }  

    public MeshInstance3D DrawLine3D(Vector3 from, Vector3 to, Color? color = null)
    {
        if (EngineDebugger.IsActive() == false)
            return null;

        var meshInstance = new MeshInstance3D();
        var immediateMesh = new ImmediateMesh();
        var material = new StandardMaterial3D();

        meshInstance.Mesh = immediateMesh;
        meshInstance.CastShadow = GeometryInstance3D.ShadowCastingSetting.Off;

        immediateMesh.SurfaceBegin(Mesh.PrimitiveType.Lines, material);
        immediateMesh.SurfaceAddVertex(from);
        immediateMesh.SurfaceAddVertex(to);
        immediateMesh.SurfaceEnd();

        material.ShadingMode = StandardMaterial3D.ShadingModeEnum.Unshaded;
        material.AlbedoColor = color ?? _defaultColor;

        _node.AddChild(meshInstance);

        return meshInstance;
    }

    public MeshInstance3D DrawPoint3D(Vector3 pos, float radius = 0.05f, Color? color = null)
    {
        if (EngineDebugger.IsActive() == false)
            return null;

        var meshInstance = new MeshInstance3D();
        var sphereMesh = new SphereMesh();
        var material = new StandardMaterial3D();

        meshInstance.Mesh = sphereMesh;
        meshInstance.CastShadow = GeometryInstance3D.ShadowCastingSetting.Off;
        meshInstance.Position = pos;

        sphereMesh.Radius = radius;
        sphereMesh.Height = radius * 2f;
        sphereMesh.Material = material;

        material.ShadingMode = StandardMaterial3D.ShadingModeEnum.Unshaded;
        material.AlbedoColor = color ?? _defaultColor;

        _node.AddChild(meshInstance);

        return meshInstance;
    }

    public Line2D DrawLine2D( Vector2 from, Vector2 to, float width = 2.0f , Color? color = null)
    {
        if (EngineDebugger.IsActive() == false)
            return null;

        Line2D drawer = new Line2D();
        drawer.Width = width;

        if (color == null)
            drawer.DefaultColor = _defaultColor;
        else
            drawer.DefaultColor = (Godot.Color)color;
        drawer.AddPoint(from);
        drawer.AddPoint(to);
        _node.AddChild(drawer);

        return drawer;
    }
}
