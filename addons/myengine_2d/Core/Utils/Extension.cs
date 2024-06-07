using Define;
using Godot;
using Godot.Collections;
using System;
using System.Drawing;
using System.Reflection;
using System.Text;

public static class Extension 
{
    #region String
    public static string ToSnakeCase(this string text)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }
        if (text.Length < 2)
        {
            return text.ToLowerInvariant();
        }
        var sb = new StringBuilder();
        sb.Append(char.ToLowerInvariant(text[0]));
        for (int i = 1; i < text.Length; ++i)
        {
            char c = text[i];
            if (char.IsUpper(c))
            {
                sb.Append('_');
                sb.Append(char.ToLowerInvariant(c));
            }
            else
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }
    #endregion

    #region GodotObject
    /// <summary>
    /// return false if it is null || NotInstanceValid || QueuedFree(Node Only)
    /// </summary>
    public static bool IsValid<T>(this T obj) where T : GodotObject
    {
        if (obj is Node objT)
        {
            return obj != null
            && GodotObject.IsInstanceValid(obj)
            && !obj.IsQueuedForDeletion();
        }
        else
        {
            return obj != null
            && GodotObject.IsInstanceValid(obj);
        }
    }
    #endregion

    #region Node

    public static void AddChildDeferred( this Node node, Node child, bool ReadableName = false, Node.InternalMode @internal = Node.InternalMode.Disabled) 
    {
        Variant[] args = {child,  ReadableName, (int)@internal};
        node.CallDeferred(Node.MethodName.AddChild, args);
    }

    public static void QueueFreeDeferred(this Node node)
    {
        node.CallDeferred(Node.MethodName.QueueFree);
    }

    public static SignalAwaiter WaitForSeconds(this Node node, double timeSec, bool processAlways = true, bool processInPhysics = false, bool ignoreTimeScale = false)
    {
        return node.ToSignal(node.GetTree().CreateTimer(timeSec, processAlways, processInPhysics, ignoreTimeScale), SceneTreeTimer.SignalName.Timeout);
    }

    public static SceneTreeTimer AddSceneTreeTimer(this Node node, double timeSec, bool processAlways = true, bool processInPhysics = false, bool ignoreTimeScale = false)
    {
        return node.GetTree().CreateTimer(timeSec, processAlways, processInPhysics, ignoreTimeScale);
    }

    /// <summary>
    ///  Find or Add this Node's child with name.
    /// </summary>
    public static T GetorAddChildByName<T>(this Node parent, string name, bool recursive = true) where T : Node, new()
    {
        T child = parent.FindChild(name, recursive) as T;

        if (child != null)
            return child;
        child = new T();
        parent.AddChild(child);
        return child;
    }

    public static T GetOrAddChildByType<T>(this Node parent, bool recursive = true) where T : Node, new()
    {
        T child = GetChildByType<T>(parent, recursive);
        if (child != null)
            return child;
        child = new T();
        parent.AddChild(child);
        return child;
    }

    public static T GetOrAddChildBySceneType<T>(this Node parent, bool recursive = true) where T : Node
    {
        T child = GetChildByType<T>(parent, recursive);

        if (child != null)
            return child;

        PackedScene scn = Managers.Resource.LoadPackedScene<T>(Define.Scenes.ContentNodes);
        if(scn != null)
        child = Managers.Resource.Instantiate<T> (scn, parent);
        return child;
    }
   
    ///// <summary>
    ///// if object is not valid, return NULL and PushWarning
    ///// </summary>
    //public static T TryGetChildByType<T>(this Node node, bool recursive = true) where T : Node
    //{
    //    var obj = GetChildByType<T>(node, recursive);
    //    if (obj == null)
    //        GD.PushWarning($"GetChild Failed : {typeof(T).Name}");
    //    return obj;
    //}
    ///// <summary>
    ///// if object is not valid, return NULL and PushWarning
    ///// </summary>
    //public static T TryGetParentByType<T>(this Node node) where T : Node
    //{
    //    var obj = GetParentByType<T>(node);
    //    if (obj == null)
    //        GD.PushWarning($"GetParent Failed : {typeof(T).Name}");
    //    return obj;
    //}

    public static Array<Node> GetChildrenByType<T>(this Node node, bool recursive = true) where T : Node
    {
        Array<Node> children = new Array<Node>();

        int childCount = node.GetChildCount();

        for (int i = 0; i < childCount; i++)
        {
            Node child = node.GetChild(i);
            if (child is T childT && childT.IsValid())
            {
                children.Add(childT);
            }

            if (recursive && child.GetChildCount() > 0)
            {
                T recursiveResult = child.GetChildByType<T>(true);
                if (recursiveResult != null)
                {
                    children.Add(recursiveResult);
                }
            }
        }
        return children.Count > 0 ? children : null;
    }

    /// <summary>
    /// if object is not valid, return NULL. actslike Unity.GetCompoenet
    /// </summary>
    public static T GetChildByType<T>(this Node node, bool recursive = true) where T : Node
    {
        int childCount = node.GetChildCount();

        for (int i = 0; i < childCount; i++)
        {
            Node child = node.GetChild(i);
            if (child is T childT && childT.IsValid())
                return childT;

            if (recursive && child.GetChildCount() > 0)
            {
                T recursiveResult = child.GetChildByType<T>(true);
                if (recursiveResult != null)
                    return recursiveResult;
            }
        }

        return null;
    }

    /// <summary>
    /// if object is not valid, return NULL. acts like Unity.GetParent
    /// </summary>
    public static T GetParentByType<T>(this Node node) where T : Node
    {
        Node parent = node.GetParent();
        if (parent.IsValid())
        {
            if (parent is T parentT)
            {
                return parentT;
            }
            else
            {
                return parent.GetParentByType<T>();
            }
        }

        return null;
    }
    #endregion

    #region Vector

    public static bool Or(this Vector2I v1, Vector2I v2)
    {
        return v1.X == v2.X || v1.Y == v2.Y;
    }

    public static Vector2I RoundInt(this Vector2 vector, int size)
    {
        return new Vector2I(Utils.RoundIntSize(vector.X, size), Utils.RoundIntSize((float)vector.Y, size));
    }

    public static Vector2I ToVector2I(this Vector2 vector)
    {
        return new Vector2I((int)vector.X, (int)vector.Y);
    }

    public static Vector2 ToVector2(this Vector2I vector)
    {
        return new Vector2(vector.X, vector.Y);
    }

    public static Vector3I ToVector3I(this Vector3 vector)
    {
        return new Vector3I((int)vector.X, (int)vector.Y, (int)vector.Z);
    }

    public static DelaunatorEx.GridPoint ToGridPoint(this Vector2I vec)
    {
        return new DelaunatorEx.GridPoint { Vector = (Vector2I)vec };
    }
    #endregion

    #region CollisionShape2D

    //public Corner GetIntersectionCorner(this CollisionShape2D collision, Vector2 direction)
    //{
    //    RectangleShape2D shape =  collision.Shape as RectangleShape2D;

    //    Vector2 center = collision.GlobalPosition;
    //    float width = shape.Size.X;
    //    float height = shape.Size.Y;

    //    // Calculate the intersection point of the line with the rectangle
    //    Point intersection = GetIntersectionPoint(line, rect);

    //    // Determine which corner the line intersects
    //    if (intersection.X == center.X - width / 2 && intersection.Y == center.Y - height / 2)
    //        return Corner.TopLeft;
    //    else if (intersection.X == center.X + width / 2 && intersection.Y == center.Y - height / 2)
    //        return Corner.TopRight;
    //    else if (intersection.X == center.X - width / 2 && intersection.Y == center.Y + height / 2)
    //        return Corner.BottomLeft;
    //    else if (intersection.X == center.X + width / 2 && intersection.Y == center.Y + height / 2)
    //        return Corner.BottomRight;
    //    else
    //        throw new Exception("Line does not intersect with rectangle");
    //}

    //private static Vector2 GetIntersectionPoint(this CollisionShape2D collision, Define.CollisionShape shapeType, Vector2 direction)
    //{
    //    // Calculate the intersection point of the line with the rectangle
    //    // This implementation assumes a simple line-rectangle intersection algorithm
    //    // You may need to modify this implementation based on your specific requirements

    //    Vector2 center = collision.GlobalPosition;

    //    switch(shapeType)
    //    {
    //        case CollisionShape.Circle: 
    //        {

    //        } break;
    //        case CollisionShape.Rect:
    //        {

    //        }break;
    //        case CollisionShape.Capsule:
    //        {

    //        }break;
    //    }
    //    RectangleShape2D shape = collision.Shape as RectangleShape2D;



    //    return new Vector2 { };
    //}


    #endregion

    #region RandomNumberGenerator

    public static Vector2 RandVector(this RandomNumberGenerator rand, Vector2 min, Vector2 max)
    {
        var dx = Mathf.Abs(max.X - min.X);
        var dy = Mathf.Abs(max.Y - min.Y);

        return new Vector2(rand.Randf() * dx + min.X, rand.Randf() * dy + min.Y);
    }
    #endregion
}
