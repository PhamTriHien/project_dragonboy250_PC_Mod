using System;
using System.Collections.Generic;

/// <summary>
/// CLASS ModNextMapPathFinder: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public static class ModNextMapPathFinder
{
	private static readonly Dictionary<long, List<int>> s_PathCache = new Dictionary<long, List<int>>();

	/// <summary>
	/// Xóa bộ nhớ đệm đường đi khi đồ thị cổng dịch chuyển được cập nhật trực tiếp từ bản đồ đang đứng.
	/// </summary>
	public static void ClearCache()
	{
		s_PathCache.Clear();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ FindPath.
	/// </summary>
	public static List<int> FindPath(int startMapId, int targetMapId)
	{
		if (startMapId == targetMapId)
		{
			return new List<int>();
		}

		// Tuyến đường phụ thuộc giới tính, tiến độ nhiệm vụ và sức mạnh (nhà riêng 21/22/23, vách núi 39/40/41, Tương lai 92..103, Cold 105..110, map 40B/60B).
		Char meForKey = Char.myCharz();
		int gender = meForKey != null ? meForKey.cgender : -1;
		int taskId = (meForKey != null && meForKey.taskMaint != null) ? meForKey.taskMaint.taskId : -1;
		long cPower = (meForKey != null) ? meForKey.cPower : 0L;
		int powerTier = (cPower >= 60000000000L) ? 2 : ((cPower >= 40000000000L) ? 1 : 0);
		int taskKey = (taskId < 0) ? 63 : (taskId > 62 ? 62 : taskId);
		long cacheKey = ((long)startMapId << 40) | ((long)targetMapId << 16) | ((long)taskKey << 8) | ((long)powerTier << 4) | (byte)(gender + 1);
		if (s_PathCache.TryGetValue(cacheKey, out var cachedPath))
		{
			return new List<int>(cachedPath);
		}

		bool isColdEndpoint = (startMapId >= 105 && startMapId <= 110) || (targetMapId >= 105 && targetMapId <= 110);

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
				int myGender = gender;
				for (int i = 0; i < neighbors.Count; i++)
				{
					int next = neighbors[i];
					// Không định tuyến qua nhà đẻ hoặc vách núi offline của tộc khác (21/39: Trái Đất, 22/40: Namếc, 23/41: Xayda)
					if ((next == 21 || next == 39) && myGender != 0 && targetMapId != next) continue;
					if ((next == 22 || next == 40) && myGender != 1 && targetMapId != next) continue;
					if ((next == 23 || next == 41) && myGender != 2 && targetMapId != next) continue;

					// Ràng buộc nhiệm vụ Tương lai (92..103 yêu cầu taskId > 24) và Cold (105..110 yêu cầu taskId > 30, không mượn đường Cold khi đi nội bộ Fide/Xayda)
					if (next >= 92 && next <= 103 && taskId >= 0 && taskId <= 24 && targetMapId != next) continue;
					if (next >= 105 && next <= 110)
					{
						if (taskId >= 0 && taskId <= 30 && targetMapId != next) continue;
						if (!isColdEndpoint) continue;
					}

					// Ràng buộc sức mạnh tối thiểu (153..159: 40 tỷ, 155/166: 60 tỷ)
					if (cPower > 0L)
					{
						if (next >= 153 && next <= 159 && cPower < 40000000000L && targetMapId != next) continue;
						if ((next == 155 || next == 166) && cPower < 60000000000L && targetMapId != next) continue;
					}

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

		if (path != null && s_PathCache.Count < 500)
		{
			s_PathCache[cacheKey] = new List<int>(path);
		}

		return path;
	}
}
