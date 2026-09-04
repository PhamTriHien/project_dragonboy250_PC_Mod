using System;
using System.Collections.Generic;

public static class ModNextMapPathFinder
{
	public static List<int> FindPath(int startMapId, int targetMapId)
	{
		if (startMapId == targetMapId)
		{
			return new List<int>();
		}

		Queue<int> queue = new Queue<int>();
		Dictionary<int, int> parent = new Dictionary<int, int>();
		HashSet<int> visited = new HashSet<int>();

		queue.Enqueue(startMapId);
		visited.Add(startMapId);

		bool found = false;
		while (queue.Count > 0)
		{
			int curr = queue.Dequeue();
			if (curr == targetMapId)
			{
				found = true;
				break;
			}

			if (ModNextMapData.mapWaypoints.ContainsKey(curr))
			{
				List<int> neighbors = ModNextMapData.mapWaypoints[curr];
				for (int i = 0; i < neighbors.Count; i++)
				{
					int next = neighbors[i];
					if (!visited.Contains(next))
					{
						visited.Add(next);
						parent[next] = curr;
						queue.Enqueue(next);
					}
				}
			}
		}

		if (!found)
		{
			return null;
		}

		List<int> path = new List<int>();
		int currNode = targetMapId;
		int maxSteps = 100;
		while (currNode != startMapId && maxSteps-- > 0 && parent.ContainsKey(currNode))
		{
			path.Insert(0, currNode);
			currNode = parent[currNode];
		}
		return path;
	}
}
