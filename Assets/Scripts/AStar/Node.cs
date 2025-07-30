using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.AStar
{
    public class Node : IComparable<Node>
    {
        public Vector2Int gridPosition;
        public int gCost = 0;
        public int hCost = 0;
        public int FCost => gCost + hCost;
        public bool isObstacle = false;
        public Node parentNode;
        public Node(Vector2Int pos)
        {
            gridPosition = pos;
            parentNode = null;
        }

        public int CompareTo(Node other)
        {
            //比较选出最低的F值，返回-1，0，-1
            int result = FCost.CompareTo(other.FCost);
            if(result == 0)
            {
                result = hCost.CompareTo(other.hCost);
            }
            return result;
        }
    }
}