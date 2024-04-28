using Godot;
using System;
using System.Collections.Generic;

public class Utils
{
    public static T GetChildByType<T>(Node parent, string name=null, bool recursive = false) where T : Node
    {
        if(!parent.IsValid())
            return null;
        return parent.GetChildByType<T>(recursive);
    }
}
