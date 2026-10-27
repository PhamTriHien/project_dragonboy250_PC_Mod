using System;
using System.Collections.Generic;

/// <summary>
/// Thuật toán tìm đường A* 8 hướng trên lưới ô gạch 24px (TileMap) dùng cho tiếp cận quái/Boss an toàn vượt chướng ngại vật.
/// </summary>
public static class ModAStarPathFinder
{
	private sealed class PathNode
	{
		public int x;
		public int y;
		public int g;
		public int h;
		public int heapIndex;
		public PathNode parent;

		public int F => g + h;

		public PathNode(int x, int y)
		{
			this.x = x;
			this.y = y;
			this.heapIndex = -1;
		}
	}

	private sealed class MinBinaryHeap
	{
		private readonly List<PathNode> items = new List<PathNode>(256);

		public int Count => items.Count;

		public void Clear()
		{
			items.Clear();
		}

		public void Push(PathNode node)
		{
			node.heapIndex = items.Count;
			items.Add(node);
			SiftUp(node.heapIndex);
		}

		public PathNode Pop()
		{
			if (items.Count == 0)
			{
				return null;
			}
			PathNode root = items[0];
			int lastIdx = items.Count - 1;
			PathNode last = items[lastIdx];
			items.RemoveAt(lastIdx);
			if (items.Count > 0)
			{
				last.heapIndex = 0;
				items[0] = last;
				SiftDown(0);
			}
			root.heapIndex = -1;
			return root;
		}

		public void UpdatePriority(PathNode node)
		{
			if (node.heapIndex >= 0 && node.heapIndex < items.Count)
			{
				SiftUp(node.heapIndex);
			}
		}

		private void SiftUp(int index)
		{
			while (index > 0)
			{
				int parent = (index - 1) >> 1;
				if (Compare(items[index], items[parent]) >= 0)
				{
					break;
				}
				Swap(index, parent);
				index = parent;
			}
		}

		private void SiftDown(int index)
		{
			int count = items.Count;
			while (true)
			{
				int left = (index << 1) + 1;
				int right = left + 1;
				int smallest = index;
				if (left < count && Compare(items[left], items[smallest]) < 0)
				{
					smallest = left;
				}
				if (right < count && Compare(items[right], items[smallest]) < 0)
				{
					smallest = right;
				}
				if (smallest == index)
				{
					break;
				}
				Swap(index, smallest);
				index = smallest;
			}
		}

		private void Swap(int i, int j)
		{
			PathNode a = items[i];
			PathNode b = items[j];
			a.heapIndex = j;
			b.heapIndex = i;
			items[i] = b;
			items[j] = a;
		}

		private static int Compare(PathNode a, PathNode b)
		{
			int fa = a.F;
			int fb = b.F;
			if (fa != fb)
			{
				return fa - fb;
			}
			return a.h - b.h;
		}
	}

	private const int SolidTileMask = TileMap.T_TOP | TileMap.T_CENTER | TileMap.T_LEFT | TileMap.T_RIGHT | TileMap.T_SOLIDGROUND | TileMap.T_BOTTOM | TileMap.T_DIE;

	private static readonly int[] DirX = new int[8] { 0, 1, 0, -1, 1, 1, -1, -1 };
	private static readonly int[] DirY = new int[8] { -1, 0, 1, 0, -1, 1, 1, -1 };
	private static readonly int[] StepCosts = new int[8] { 10, 10, 10, 10, 14, 14, 14, 14 };

	public static int stepSize = 3;
	public static int stepDelayMs = 30;

	private static readonly MinBinaryHeap openHeap = new MinBinaryHeap();
	private static readonly Dictionary<int, PathNode> openLookup = new Dictionary<int, PathNode>(256);
	private static readonly HashSet<int> closedSet = new HashSet<int>();

	private static List<PathNode> activePath = null;
	private static int activePathIndex = 0;
	private static int activePathMapId = -1;
	private static int activeGoalTx = -1;
	private static int activeGoalTy = -1;
	private static long lastStepTick = 0;
	private static bool reopenUiAfterInput = false;

	private sealed class ConfigActionListener : IActionListener
	{
		public void perform(int idAction, object p)
		{
			try
			{
				switch (idAction)
				{
					case 10:
						PromptStepSize();
						break;
					case 11:
						PromptStepDelay();
						break;
					case 20:
						if (GameCanvas.inputDlg != null && GameCanvas.inputDlg.tfInput != null)
						{
							string raw = (GameCanvas.inputDlg.tfInput.getText() ?? string.Empty).Trim();
							if (int.TryParse(raw, out int s))
							{
								if (s < 1) s = 1;
								if (s > 3) s = 3;
								stepSize = s;
								ModConfig.SaveConfig();
								GameScr.info1?.addInfo("Bước nhảy A*: " + stepSize + " ô", 0);
							}
						}
						GameCanvas.endDlg();
						SoundMn.gI().buttonClick();
						if (reopenUiAfterInput)
						{
							reopenUiAfterInput = false;
							ModUI.uiCustomOpen = true;
						}
						break;
					case 21:
						if (GameCanvas.inputDlg != null && GameCanvas.inputDlg.tfInput != null)
						{
							string raw2 = (GameCanvas.inputDlg.tfInput.getText() ?? string.Empty).Trim();
							if (int.TryParse(raw2, out int d))
							{
								if (d < 25) d = 25;
								if (d > 100) d = 100;
								stepDelayMs = d;
								ModConfig.SaveConfig();
								GameScr.info1?.addInfo("Độ trễ bước A*: " + stepDelayMs + "ms", 0);
							}
						}
						GameCanvas.endDlg();
						SoundMn.gI().buttonClick();
						if (reopenUiAfterInput)
						{
							reopenUiAfterInput = false;
							ModUI.uiCustomOpen = true;
						}
						break;
				}
			}
			catch
			{
				GameCanvas.endDlg();
			}
		}
	}

	private static readonly ConfigActionListener configListener = new ConfigActionListener();

	public static void ShowConfigMenu()
	{
		if (GameCanvas.menu == null) return;
		reopenUiAfterInput = ModUI.uiCustomOpen;
		ModUI.uiCustomOpen = false;
		ModMenu.modMenuOpen = false;
		MyVector items = new MyVector();
		items.addElement(new Command("Bước nhảy A*: " + stepSize + " ô (1-3)", configListener, 10, null));
		items.addElement(new Command("Độ trễ bước: " + stepDelayMs + "ms (25-100)", configListener, 11, null));
		GameCanvas.menu.startAt(items, 3);
	}

	private static void PromptStepSize()
	{
		if (GameCanvas.inputDlg == null) return;
		GameCanvas.inputDlg.show("Bước nhảy A* (1 - 3 ô):", new Command("OK", configListener, 20, null), TField.INPUT_TYPE_NUMERIC);
		if (GameCanvas.inputDlg.tfInput != null)
		{
			GameCanvas.inputDlg.tfInput.setMaxTextLenght(2);
			GameCanvas.inputDlg.tfInput.setText(stepSize.ToString());
			if (!Main.isPC) GameCanvas.inputDlg.tfInput.setFocusWithKb(isFocus: true);
		}
	}

	private static void PromptStepDelay()
	{
		if (GameCanvas.inputDlg == null) return;
		GameCanvas.inputDlg.show("Độ trễ mỗi bước A* (25 - 100 ms):", new Command("OK", configListener, 21, null), TField.INPUT_TYPE_NUMERIC);
		if (GameCanvas.inputDlg.tfInput != null)
		{
			GameCanvas.inputDlg.tfInput.setMaxTextLenght(4);
			GameCanvas.inputDlg.tfInput.setText(stepDelayMs.ToString());
			if (!Main.isPC) GameCanvas.inputDlg.tfInput.setFocusWithKb(isFocus: true);
		}
	}

	public static void ResetPath()
	{
		activePath = null;
		activePathIndex = 0;
		activePathMapId = -1;
		activeGoalTx = -1;
		activeGoalTy = -1;
		lastStepTick = 0;
	}

	private static int PackKey(int tx, int ty)
	{
		return ty * TileMap.tmw + tx;
	}

	private static int ComputeOctileHeuristic(int x1, int y1, int x2, int y2)
	{
		int dx = Res.abs(x1 - x2);
		int dy = Res.abs(y1 - y2);
		int minD = (dx < dy) ? dx : dy;
		return (dx + dy) * 10 - minD * 6;
	}

	private static bool IsWalkableTile(int tx, int ty, int goalTx, int goalTy)
	{
		if (tx < 0 || ty < 0 || tx >= TileMap.tmw || ty >= TileMap.tmh)
		{
			return false;
		}
		if ((TileMap.tileTypeAt(tx, ty) & SolidTileMask) != 0)
		{
			return false;
		}
		if (ty > 0 && (TileMap.tileTypeAt(tx, ty - 1) & SolidTileMask) != 0)
		{
			return false;
		}
		if (Res.abs(tx - goalTx) > 1 || Res.abs(ty - goalTy) > 1)
		{
			int px = tx * 24 + 12;
			int py = ty * 24 + 24;
			if (ModTanSatTargeting.IsInsideWaypoint(px, py))
			{
				return false;
			}
		}
		return true;
	}

	private static bool ResolveNearestWalkableTile(int rawTx, int rawTy, int goalTx, int goalTy, out int outTx, out int outTy)
	{
		int clampedX = rawTx < 0 ? 0 : (rawTx >= TileMap.tmw ? TileMap.tmw - 1 : rawTx);
		int clampedY = rawTy < 0 ? 0 : (rawTy >= TileMap.tmh ? TileMap.tmh - 1 : rawTy);
		if (IsWalkableTile(clampedX, clampedY, goalTx, goalTy))
		{
			outTx = clampedX;
			outTy = clampedY;
			return true;
		}
		for (int r = 1; r <= 5; r++)
		{
			for (int dx = -r; dx <= r; dx++)
			{
				for (int dy = -r; dy <= r; dy++)
				{
					if ((Res.abs(dx) == r || Res.abs(dy) == r) && IsWalkableTile(clampedX + dx, clampedY + dy, goalTx, goalTy))
					{
						outTx = clampedX + dx;
						outTy = clampedY + dy;
						return true;
					}
				}
			}
		}
		outTx = clampedX;
		outTy = clampedY;
		return false;
	}

	private static List<PathNode> FindPath(int startPx, int startPy, int goalPx, int goalPy)
	{
		if (TileMap.tmw <= 0 || TileMap.tmh <= 0 || TileMap.types == null)
		{
			return null;
		}

		int rawStartTx = startPx / 24;
		int rawStartTy = (startPy - 1) / 24;
		int rawGoalTx = goalPx / 24;
		int rawGoalTy = (goalPy - 1) / 24;

		if (!ResolveNearestWalkableTile(rawGoalTx, rawGoalTy, rawGoalTx, rawGoalTy, out int gx, out int gy))
		{
			return null;
		}
		if (!ResolveNearestWalkableTile(rawStartTx, rawStartTy, gx, gy, out int sx, out int sy))
		{
			return null;
		}

		openHeap.Clear();
		openLookup.Clear();
		closedSet.Clear();

		PathNode startNode = new PathNode(sx, sy);
		startNode.g = 0;
		startNode.h = ComputeOctileHeuristic(sx, sy, gx, gy);
		openHeap.Push(startNode);
		openLookup[PackKey(sx, sy)] = startNode;

		int iterations = 0;
		const int maxIterations = 2400;

		while (openHeap.Count > 0 && iterations < maxIterations)
		{
			iterations++;
			PathNode current = openHeap.Pop();
			int curKey = PackKey(current.x, current.y);
			openLookup.Remove(curKey);

			if (current.x == gx && current.y == gy)
			{
				List<PathNode> result = new List<PathNode>();
				PathNode trace = current;
				while (trace != null)
				{
					result.Add(trace);
					trace = trace.parent;
				}
				result.Reverse();
				return result;
			}

			closedSet.Add(curKey);

			for (int i = 0; i < 8; i++)
			{
				int nx = current.x + DirX[i];
				int ny = current.y + DirY[i];
				if (!IsWalkableTile(nx, ny, gx, gy))
				{
					continue;
				}
				if (i >= 4 && (!IsWalkableTile(current.x + DirX[i], current.y, gx, gy) || !IsWalkableTile(current.x, current.y + DirY[i], gx, gy)))
				{
					continue;
				}

				int nKey = PackKey(nx, ny);
				if (closedSet.Contains(nKey))
				{
					continue;
				}

				int tentativeG = current.g + StepCosts[i];
				if (!openLookup.TryGetValue(nKey, out PathNode neighbor))
				{
					neighbor = new PathNode(nx, ny);
					neighbor.g = tentativeG;
					int baseH = ComputeOctileHeuristic(nx, ny, gx, gy);
					int cross = Res.abs((nx - gx) * (sy - gy) - (sx - gx) * (ny - gy)) >> 3;
					neighbor.h = baseH + cross;
					neighbor.parent = current;
					openHeap.Push(neighbor);
					openLookup[nKey] = neighbor;
				}
				else if (tentativeG < neighbor.g)
				{
					neighbor.g = tentativeG;
					neighbor.parent = current;
					openHeap.UpdatePriority(neighbor);
				}
			}
		}

		return null;
	}

	public static int GetPathTileCost(int fromPx, int fromPy, int toPx, int toPy)
	{
		List<PathNode> path = FindPath(fromPx, fromPy, toPx, toPy);
		if (path == null || path.Count == 0)
		{
			return (Res.abs(fromPx - toPx) + Res.abs(fromPy - toPy)) / 24 + 40;
		}
		return path.Count;
	}

	/// <summary>
	/// Di chuyển từng bước theo đường đi A* 8 hướng tới (targetPx, targetPy). Trả về true khi đã áp sát đích.
	/// </summary>
	public static bool StepTowards(int targetPx, int targetPy)
	{
		Char me = Char.myCharz();
		if (me == null)
		{
			ResetPath();
			return false;
		}

		if (Res.abs(me.cx - targetPx) <= 20 && Res.abs(me.cy - targetPy) <= 24)
		{
			ResetPath();
			return true;
		}

		long now = mSystem.currentTimeMillis();
		int delay = stepDelayMs < 25 ? 25 : (stepDelayMs > 100 ? 100 : stepDelayMs);
		if (now - lastStepTick < delay)
		{
			return false;
		}

		int goalTx = targetPx / 24;
		int goalTy = (targetPy - 1) / 24;
		bool needRecompute = activePath == null ||
		                     activePathMapId != TileMap.mapID ||
		                     activePathIndex >= activePath.Count ||
		                     Res.abs(activeGoalTx - goalTx) > 1 ||
		                     Res.abs(activeGoalTy - goalTy) > 1;

		if (needRecompute)
		{
			activePath = FindPath(me.cx, me.cy, targetPx, targetPy);
			activePathMapId = TileMap.mapID;
			activeGoalTx = goalTx;
			activeGoalTy = goalTy;
			activePathIndex = 0;
		}

		if (activePath == null || activePath.Count == 0)
		{
			lastStepTick = now;
			me.moveTo(targetPx, targetPy, 0);
			return false;
		}

		int step = stepSize < 1 ? 1 : (stepSize > 3 ? 3 : stepSize);
		int nextIdx = activePathIndex + step;
		bool isFinalStep = false;
		if (nextIdx >= activePath.Count - 1)
		{
			nextIdx = activePath.Count - 1;
			isFinalStep = true;
		}

		PathNode node = activePath[nextIdx];
		int destX = isFinalStep ? targetPx : (node.x * 24 + 12);
		int destY = isFinalStep ? targetPy : (node.y * 24 + 24);

		me.currentMovePoint = null;
		if (me.vMovePoints != null)
		{
			me.vMovePoints.removeAllElements();
		}
		ModTeleport.SendSingleCharMove(destX, destY);

		activePathIndex = nextIdx;
		lastStepTick = now;

		if (isFinalStep || (Res.abs(me.cx - targetPx) <= 20 && Res.abs(me.cy - targetPy) <= 24))
		{
			ResetPath();
			return true;
		}

		return false;
	}
}
