using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Node
{
    public Vector2Int coordinate;
    public bool isWalkable;
    public bool isExplored;
    public bool isPath;
    public Node connectedTo;

    public Node(Vector2Int coordinates, bool isWallkable)
    {
        this.coordinate = coordinates;
        this.isWalkable = isWallkable;
    }
}
