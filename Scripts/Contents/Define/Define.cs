using DelaunatorSharp;
using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Define;

public enum Scenes
{
    CoreNodes,
    ContentNodes,
    GameScenes,
}

public enum RoomTypes
{
    Basic           = 0,
    A,
    B,
}

public enum Physics2D : uint
{
    Player          =  1 << 0,
    Monster         =  1 << 1,
    Attack          =  1 << 2,
    UI              =  1 << 3,
    DungeonRoom     =  1 << 4,
}
