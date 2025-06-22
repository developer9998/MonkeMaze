using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MonkeMaze.Generation
{
    internal struct MazeBlueprint
    {
        internal int endX, endY;
        internal int scale;
        internal bool[,] blueprint;
    }

    internal enum Direction
    {
        up,
        down,
        left,
        right,
    }

    internal static class RecursiveBacktracker
    {
        internal static MazeBlueprint Blueprint;

        internal static Dictionary<Direction, int[]> Dir2Points = new()
        {
            { Direction.up    , new int[] {0  ,  1} },
            { Direction.down  , new int[] {0  , -1} },
            { Direction.left  , new int[] {-1 ,  0} },
            { Direction.right , new int[] {1  ,  0} }
        };

        internal static Dictionary<Direction, Direction> reverseDir = new()
        {
            { Direction.up    , Direction.down  },
            { Direction.down  , Direction.up    },
            { Direction.left  , Direction.right },
            { Direction.right , Direction.left  }
        };

        internal static Direction[] alLDirs =
        [
            Direction.up, Direction.down, Direction.left, Direction.right
        ];

        internal static List<int[]> DeadEnds = [];
        internal static void GenBlueprint(int scale)
        {
            Blueprint = new MazeBlueprint
            {
                scale = scale,
                blueprint = new bool[scale, scale]
            };
            for (int x = 0; x < scale; ++x)
            {
                for (int y = 0; y < scale; ++y)
                {
                    Blueprint.blueprint[x, y] = false;
                }
            }
            int middle = Mathf.FloorToInt(scale / 2);
            Blueprint.blueprint[middle, middle] = true;
            Blueprint.blueprint[middle, middle + 1] = true;
            int[] pos = [middle, middle + 2];
            Search(pos, Direction.up);

            for (int x = 0; x < scale; ++x)
            {
                for (int y = 0; y < scale; ++y)
                {
                    int[] tile = [x, y];
                    if (!GetPoint(tile)) break;
                    if (tile == new int[] { middle, middle }) break;
                    int count = 0;
                    foreach (Direction dir in alLDirs)
                    {
                        int[] check = MoveDir(tile, dir, 1);
                        if (!CheckExists(check)) continue;
                        if (GetPoint(check)) ++count;
                    }
                    if (count == 1) DeadEnds.Add(tile);
                }
            }

            int[] ending = DeadEnds[Rng.RandIndex(DeadEnds.Count)];
            Blueprint.endX = ending[0]; Blueprint.endY = ending[1];
        }
        internal static void DebugDraw()
        {
            string draw = "\n";
            for (int x = 0; x < Blueprint.scale; ++x)
            {
                for (int y = 0; y < Blueprint.scale; ++y)
                {
                    draw += Blueprint.blueprint[x, y] ? " " : "#";
                }
                draw += "\n";
            }
            Plugin.Logger.LogInfo(draw);
        }
        internal static bool GetPoint(int[] pos)
        {
            if (!CheckExists(pos)) return false;
            return Blueprint.blueprint[pos[0], pos[1]];
        }
        internal static void SetPoint(int[] pos, bool value)
        {
            if (!CheckExists(pos)) return;
            Blueprint.blueprint[pos[0], pos[1]] = value;
        }

        internal static void Search(int[] pos, Direction entrance)
        {
            bool closed = false;
            int iter = 0;
            int[] bridge = MoveDir(pos, reverseDir[entrance], 1);
            SetPoint(pos, true);
            SetPoint(bridge, true);
            while (!closed)
            {
                //Console.WriteLine($"{pos[0]} , {pos[1]}");
                //DebugDraw();
                List<Direction> dirs = [.. alLDirs];
                List<Direction> remove = [reverseDir[entrance]];
                foreach (Direction dir in dirs)
                {
                    int[] check = MoveDir(pos, dir, 2);
                    if (!CheckEmpty(check)) { remove.Add(dir); continue; }
                    if (GetPoint(check)) { remove.Add(dir); }
                }
                remove.ForEach(dir => dirs.Remove(dir));
                //dirs.ForEach(dir => Console.WriteLine(dir));
                if (dirs.Count == 0) closed = true;
                if (closed) return;

                Direction newDir = dirs[Rng.RandIndex(dirs.Count)];
                int[] next = MoveDir(pos, newDir, 2);
                Search(next, newDir);
                ++iter;
            }
        }

        internal static bool CheckExists(int[] index)
        {
            if (Blueprint.scale <= index[0] || index[0] < 0) return false;
            if (Blueprint.scale <= index[1] || index[1] < 0) return false;
            return true;
        }
        internal static bool CheckEmpty(int[] index)
        {
            if (!CheckExists(index)) return false;
            List<Direction> dirs = [.. alLDirs];
            for (int x = -1; x < 2; ++x)
            {
                for (int y = -1; y < 2; ++y)
                {
                    int[] checkIndex = (int[])index.Clone();
                    checkIndex[0] += x;
                    checkIndex[1] += y;
                    if (!CheckExists(checkIndex)) continue;
                    if (GetPoint(checkIndex)) return false;
                }
            }
            return true;
        }

        internal static int[] MoveDir(int[] index, Direction dir, int dist)
        {
            int[] newIndex = (int[])index.Clone();
            newIndex[0] += Dir2Points[dir][0] * dist;
            newIndex[1] += Dir2Points[dir][1] * dist;
            return newIndex;
        }
    }
}
