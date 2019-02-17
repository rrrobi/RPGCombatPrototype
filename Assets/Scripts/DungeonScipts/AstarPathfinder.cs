using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Node
{
    public int H_Value; // Heuristic - distance to target
    public int G_Value; // Move cost
    public int F_Value; // G + H
    public Node Parent; // Node used to reach this node
    public int xPos;
    public int yPos;

    public string Name { get { return xPos.ToString() + "_" + yPos.ToString(); } }

    // Overload for start node, as it does not have a parent
    public Node(int x, int y, Node target)
    {
        xPos = x;
        yPos = y;
        H_Value = FindHeuristic(this, target);
        G_Value = 0;
        F_Value = H_Value + G_Value;
    }

    // Overload for Target node
    public Node(int x, int y)
    {
        xPos = x;
        yPos = y;
        H_Value = 0;
        G_Value = 0;
        F_Value = H_Value + G_Value;
    }

    public Node(int x, int y, Node target, int moveCost, Node parent)
    {
        xPos = x;
        yPos = y;
        H_Value = FindHeuristic(this, target);
        Parent = parent;
        G_Value = parent.G_Value + moveCost;
        F_Value = H_Value + G_Value;
    }

    int FindHeuristic(Node start, Node target)
    {
        int startXIndex = start.xPos;
        int startYIndex = start.yPos;

        int targetXIndex = target.xPos;
        int targetYIndex = target.yPos;

        int H = Mathf.Abs(targetXIndex - startXIndex) + Mathf.Abs(targetYIndex - startYIndex);

        return H;
    }
}

public class AstarPathfinder 
{
    int[,] Map;
    int MAP_WIDTH = 10;
    int MAP_HEIGHT = 10;
    Dictionary<int, int> CostModList;
    List<int> ImpassableList;
    Node StartNode;
    Node TargetNode;

    Dictionary<string, Node> openList = new Dictionary<string, Node>();
    List<Node> closedList = new List<Node>();
    List<Node> path = new List<Node>();
    bool foundTarget = false;

    public AstarPathfinder(int[,] map,
        Dictionary<int, int> costModList, List<int> impassable)
    {
        Map = map;
        MAP_WIDTH = map.GetLength(0);
        MAP_HEIGHT = map.GetLength(1);
        CostModList = costModList;
        ImpassableList = impassable;
    }

    public List<Node> StartPathfinder(Vector2Int startIndex, Vector2Int targetIndex)
    {
        // Reset pathfinder
        foundTarget = false;
        closedList = new List<Node>();
        openList = new Dictionary<string, Node>();
        TargetNode = new Node(targetIndex.x, targetIndex.y);
        StartNode = new Node(startIndex.x, startIndex.y, TargetNode);

        // reset current path
        path = new List<Node>();

        // Add Start node to ClosedList
        AddToClosedList(StartNode);

        // Add all surrounding nodes to OpenList
        Find_Check_AddSurroundingNodesToOpenList(StartNode);

        while (foundTarget == false)
        {
            // Find smallest F value on OpenList,
            int lowestF = openList.Min(s => s.Value.F_Value);
            Node lowestFNode = openList.First(s => s.Value.F_Value == lowestF).Value;
            // Add to ClosedList
            RemoveFromOpenList(lowestFNode);
            AddToClosedList(lowestFNode);
            // Add surrounding nodes to Open list
            // If node is the target, stop, we've found the end!
            // If node is already on closed list, ignore
            // If node is already on Open list, check if new F value would be lower than previous - if so update node info
            Find_Check_AddSurroundingNodesToOpenList(lowestFNode);
        }

        Debug.Log("Path found!");
        BuildPath();

        return path;
    }

    void Find_Check_AddSurroundingNodesToOpenList(Node parent)
    {
        //  We are only interested in horizontal and virtical, NOT diagonal
        int newX = parent.xPos;
        int newY = parent.yPos;

        Node newNode;
        // Check Left
        ///////////////
        // Get pos
        newX = parent.xPos - 1;
        newY = parent.yPos;
        if (IsNodeInMap(newX, newY))
        {
            newNode = new Node(newX, newY, TargetNode, CostModList[Map[newX, newY]], parent);
            if (CheckNode(newNode))
            {
                foundTarget = true;
            }
        }
        // Check Right
        ///////////////
        // Get pos
        newX = parent.xPos + 1;
        newY = parent.yPos;
        if (IsNodeInMap(newX, newY))
        {
            newNode = new Node(newX, newY, TargetNode, CostModList[Map[newX, newY]], parent);
            if (CheckNode(newNode))
            { foundTarget = true; }
        }
        // Check Up
        ///////////////
        // Get pos
        newX = parent.xPos;
        newY = parent.yPos + 1;
        if (IsNodeInMap(newX, newY))
        {
            newNode = new Node(newX, newY, TargetNode, CostModList[Map[newX, newY]], parent);
            if (CheckNode(newNode))
            { foundTarget = true; }
        }
        // Check Down
        ///////////////
        // Get pos
        newX = parent.xPos;
        newY = parent.yPos - 1;
        if (IsNodeInMap(newX, newY))
        {
            newNode = new Node(newX, newY, TargetNode, CostModList[Map[newX, newY]], parent);
            if (CheckNode(newNode))
            { foundTarget = true; }
        }
    }

    bool IsNodeInMap(int x, int y)
    {
        if (x < 0 || x >= MAP_WIDTH ||
            y < 0 || y >= MAP_HEIGHT)
            return false;

        return true;
    }

    bool CheckNode(Node newNode)
    {
        // If tile is impassable - Ignore
        if (ImpassableList.Contains(Map[newNode.xPos, newNode.yPos]))
        { }
        // If Node is the target - Break out, no need to keep looking
        else if (TargetNode.Name == newNode.Name)
        {
            TargetNode = newNode;
            return true;
        }
        // If Node is in the ClosedList - Ignore, we have already checked this
        else if (CheckClosedListForNode(newNode))
        { }
        // If Node is already in the OpenList - check if info needs to be updated
        else if (CheckOpenListForNode(newNode))
        {
            if (newNode.F_Value < openList[newNode.Name].F_Value)
            {
                openList[newNode.Name] = newNode;
            }
        }
        // Add to OpenList
        else
        {
            openList.Add(newNode.Name, newNode);
        }

        // We have not yet found the target
        return false;
    }

    bool CheckClosedListForNode(Node node)
    {
        foreach (var item in closedList)
        {
            if (item.Name == node.Name)
                return true;
        }
        return false;
    }

    bool CheckOpenListForNode(Node node)
    {
        if (openList.ContainsKey(node.Name))
            return true;
        else
            return false;
    }

    void RemoveFromOpenList(Node node)
    {
        if (openList.Remove(node.Name))
        {
        }
        else
            Debug.LogError(node.Name + " Could not be found in the open list.");
    }

    void BuildPath()
    {
        Node currentNode = TargetNode;
        while (currentNode.Name != StartNode.Name)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;

            // should never hit this
            if (currentNode.Parent == null)
                break;
        }
        // we have reached the start node
        path.Add(StartNode);

        path.Reverse();
    }

    void AddToClosedList(Node node)
    {
        closedList.Add(node);
    }
}
