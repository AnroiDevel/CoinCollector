using UnityEngine;

namespace CoinCollector
{
    public class Node
    {
        public Vector2 Position { get; set; }
        public float Cost { get; set; }
        public float Heuristic { get; set; }
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
