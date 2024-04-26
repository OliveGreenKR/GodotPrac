using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
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


    /// <summary>
    /// return NULL, if object is not valid.
    /// </summary>
    public static T IfValid<T>(this T obj) where T : GodotObject
    {
        return obj.IsValid() ? obj : null;
    }
    #endregion

    #region Node
    /// <summary>
    /// if object is not valid, return NULL and PushWarning
    /// </summary>
    public static T TryGetChildByType<T>(this Node node, bool recursive = true) where T : Node
    {
        var obj = GetChildByType<T>(node, recursive);
        if (obj == null)
            GD.PushWarning($"GetChild Failed : {typeof(T).Name}");
        return obj;
    }
    /// <summary>
    /// if object is not valid, return NULL and PushWarning
    /// </summary>
    public static T TryGetParentByType<T>(this Node node) where T : Node
    {
        var obj = GetParentByType<T>(node);
        if (obj == null)
            GD.PushWarning($"GetParent Failed : {typeof(T).Name}");
        return obj;
    }

    public static List<T> GetChildrenByType<T>(this Node node, bool recursive = true) where T : Node
    {
        List<T> children = new List<T>();

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
}