namespace Logic;

public class Graph
{
    public Dictionary<int, List<int>> adj = new();

    public void AddEdge(int u, int v)
    {
        if (!adj.ContainsKey(u)) adj[u] = new List<int>();
        if (!adj.ContainsKey(v)) adj[v] = new List<int>();

        adj[u].Add(v);
        adj[v].Add(u);
    }

    public List<int> GetNodes()
    {
        var set = new HashSet<int>();
        foreach (var kv in adj)
        {
            set.Add(kv.Key);
            foreach (var v in kv.Value)
                set.Add(v);
        }
        return new List<int>(set);
    }

    public List<int> GetNeighbors(int v)
    {
        return adj.ContainsKey(v) ? adj[v] : new List<int>();
    }

    public List<int> BFS(int start, int? end = null)
    {
        var visited = new HashSet<int>();
        var queue = new Queue<int>();
        var result = new List<int>();

        visited.Add(start);
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            int node = queue.Dequeue();
            result.Add(node);

            if (end.HasValue && node == end.Value)
                return result;

            foreach (var neighbor in GetNeighbors(node))
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        return result;
    }
    public List<int> DFS(int start, int? end = null)
    {
        var visited = new HashSet<int>();
        var result = new List<int>();
        bool foundEnd = false;

        DFSUtil(start, visited, result, end, ref foundEnd);
        return result;
    }

    private void DFSUtil(int node, HashSet<int> visited, List<int> result, int? end, ref bool foundEnd)
    {
        if (foundEnd) return;

        visited.Add(node);
        result.Add(node);

        if (end.HasValue && node == end.Value)
        {
            foundEnd = true;
            return;
        }

        foreach (var neighbor in GetNeighbors(node))
        {
            if (!visited.Contains(neighbor))
                DFSUtil(neighbor, visited, result, end, ref foundEnd);
        }
    }
}
