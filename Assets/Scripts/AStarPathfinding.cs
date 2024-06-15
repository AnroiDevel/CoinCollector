using System.Collections.Generic;
using UnityEngine;

namespace CoinCollector
{
    public class AStarPathfinding
    {
        public static List<Vector2> FindPath(Vector2 start, Vector2 goal)
        {
            var openList = new List<Node>();
            var closedList = new HashSet<Vector2>();
            var startNode = new Node(start, 0, Vector2.Distance(start, goal));
            openList.Add(startNode);

            int maxIterations = 1000;  // Ограничение на количество итераций
            int iterations = 0;

            while(openList.Count > 0 && iterations < maxIterations)
            {
                openList.Sort((node1, node2) => node1.TotalCost.CompareTo(node2.TotalCost));
                var currentNode = openList[0];
                openList.RemoveAt(0);

                if(Vector2.Distance(currentNode.Position, goal) < 0.1f)
                {
                    var path = new List<Vector2>();
                    while(currentNode != null)
                    {
                        path.Add(currentNode.Position);
                        currentNode = currentNode.Parent;
                    }
                    path.Reverse();
                    return path;
                }

                closedList.Add(currentNode.Position);

                var neighbors = GetNeighbors(currentNode.Position);
                foreach(var neighbor in neighbors)
                {
                    if(closedList.Contains(neighbor))
                        continue;

                    var neighborNode = new Node(neighbor, currentNode.Cost + 1, Vector2.Distance(neighbor, goal), currentNode);
                    var openNode = openList.Find(node => node.Position == neighbor);

                    if(openNode == null)
                    {
                        openList.Add(neighborNode);
                    }
                    else if(openNode.Cost > neighborNode.Cost)
                    {
                        openNode.Cost = neighborNode.Cost;
                        openNode.Parent = neighborNode.Parent;
                    }
                }

                iterations++;
            }

            Debug.LogWarning("Path not found or too many iterations!");
            return null;
        }

        private static List<Vector2> GetNeighbors(Vector2 position)
        {
            var neighbors = new List<Vector2>();
            var directions = new Vector2[]
            {
                new Vector2(1, 0),
                new Vector2(-1, 0),
                new Vector2(0, 1),
                new Vector2(0, -1)
            };

            foreach(var direction in directions)
            {
                Vector2 neighborPos = position + direction;
                RaycastHit2D hit = Physics2D.Raycast(position, direction, 1f);
                if(hit.collider == null || !hit.collider.CompareTag("Obstacle"))
                {
                    neighbors.Add(neighborPos);
                }
            }

            return neighbors;
        }

        private class Node
        {
            public Vector2 Position { get; }
            public float Cost { get; set; }
            public float Heuristic { get; }
            public Node Parent { get; set; }

            public Node(Vector2 position, float cost, float heuristic, Node parent = null)
            {
                Position = position;
                Cost = cost;
                Heuristic = heuristic;
                Parent = parent;
            }

            public float TotalCost => Cost + Heuristic;
        }
    }
}
