using System;
using System.Collections.Generic;
using System.Linq;

namespace AstarFindPath
{
    class Program
    {
        static void Main(string[] args)
        {
            int[,] map = new int[10, 10] {
            { 1, 1, 1, 0, 1, 1, 1, 1, 1, 1},
            { 1, 1, 1, 0, 1, 1, 1, 1, 1, 1},
            { 1, 1, 1, 0, 1, 1, 1, 1, 1, 1},
            { 1, 1, 1, 0, 1, 1, 1, 1, 1, 1},
            { 1, 1, 1, 0, 1, 1, 1, 1, 1, 1},
            { 1, 1, 1, 1, 0, 1, 1, 1, 1, 1},
            { 1, 1, 1, 0, 1, 1, 1, 1, 1, 1},
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            };
            Astar stAstar = new Astar(map);

            Node start = stAstar.stMap[1][0];
            Node end = stAstar.stMap[9][9];
            start.g = stAstar.CalcH(start, end);
            start.f = stAstar.CalcF(start);
            stAstar.openList.Add(start);
            while (stAstar.openList.Count > 0)
            {
                Node node = stAstar.FindLeastInOpenList();
                if (node == end)
                {
                    while (node != null)
                    {
                        string str = node.x + "," + node.y;
                        Console.WriteLine(str);
                        node = node.parent;
                    }
                    return;
                }

                stAstar.AddNodeToOpenlist(node, end);
                stAstar.OpenToClose(node);
            }
        }
    }
    class Astar
    {
        public const int pay1 = 10;
        public const int pay2 = 14;

        public List<Node> openList;
        public List<Node> closeList;

        public Node[][] stMap;

        public Astar(int[,] map)
        {
            stMap = new Node[map.GetLength(0)][];

            for (int i = 0; i < map.GetLength(0); i++)
            {
                stMap[i] = new Node[map.GetLength(0)];
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    Node node = new Node(i, j, map[i, j] == 0);
                    stMap[i][j] = node;
                }
            }

            openList = new List<Node>();
            closeList = new List<Node>();
        }

        public int CalcF(Node stNode)
        {
            return stNode.g + stNode.h;
        }

        //当前这个点到下一个点的代价
        public int CalcG(Node stNode1, Node stNode2)
        {
            int pay = (Math.Abs(stNode1.x - stNode2.x) + Math.Abs(stNode1.y - stNode2.y)) == 1 ? pay1 : pay2;
            return pay + stNode1.g;
        }

        //当前点到目标点
        public int CalcH(Node stNode1, Node stNode2)
        {
            //曼哈顿距离
            return (Math.Abs(stNode1.x - stNode2.x) + Math.Abs(stNode1.y - stNode2.y)) * pay1;
        }
        //把某个点的周围的点放入openlist
        public void AddNodeToOpenlist(Node stNode, Node stTargetNode)
        {
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        continue;
                    }
                    int x = stNode.x + i;
                    int y = stNode.y + j;
                    if (x < stMap.Length && y < stMap[0].Length && x >= 0 && y >= 0)
                    {
                        if (closeList.Contains(stMap[x][y]) || stMap[x][y].IsDef)
                        {
                            continue;
                        }
                        if (openList.Contains(stMap[x][y]))
                        {
                            int newG = CalcG(stNode, stMap[x][y]);
                            if (newG < stMap[x][y].g)
                            {
                                stMap[x][y].g = newG;
                                stMap[x][y].f = CalcF(stMap[x][y]);
                                stMap[x][y].parent = stNode;
                            }
                            continue;
                        }
                        stMap[x][y].h = CalcH(stMap[x][y], stTargetNode);
                        stMap[x][y].g = CalcG(stNode, stMap[x][y]);
                        stMap[x][y].f = CalcF(stMap[x][y]);
                        stMap[x][y].parent = stNode;
                        openList.Add(stMap[x][y]);
                    }
                }
            }
        }

        public void OpenToClose(Node node)
        {
            openList.Remove(node);
            closeList.Add(node);
        }

        public Node FindLeastInOpenList()
        {
            openList.OrderBy(a => a.f);
            return openList[0];
        }
    }
    class Node
    {
        public int x, y;
        public int f, g, h;
        public Node parent;
        public bool IsDef;//是否为障碍物

        public Node(int x, int y, bool isDef)
        {
            this.x = x;
            this.y = y;
            IsDef = isDef;
        }
    }
}
