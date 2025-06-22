using System.Collections.Generic;
using UnityEngine;

namespace MonkeMaze.Generation
{
    internal enum Tiles
    {
        floor,
        wall,
        spawn,
        relic,
        none
    }

    internal static class Creation
    {
        internal static Vector3 position = new(-50, 70, -70);
        internal static float size = 3f;
        internal static int scale = 32;
        internal static Tiles[,] map;
        internal static int[] start = new int[2];
        internal static bool Created = false;
        internal static GameObject MonkeMaze;

        internal static void GenerateAll()
        {
            if (!Created) { CreateMap(); SpawnMaze(); Created = true; }
            else { MonkeMaze.SetActive(true); }
        }

        internal static void Remove()
        {
            if (!Created) return;
            Object.Destroy(MonkeMaze);
            Created = false;
        }
        internal static void CreateMap()
        {
            map = new Tiles[scale + 2, scale + 2];
            for (int i = 0; i < scale + 2; ++i)
            {
                for (int j = 0; j < scale + 2; ++j)
                {
                    map[i, j] = Tiles.wall;
                }
            }

            RecursiveBacktracker.GenBlueprint(scale);
            for (int x = 0; x < scale; x++)
            {
                for (int y = 0; y < scale; y++)
                {
                    bool isFloor = RecursiveBacktracker.Blueprint.blueprint[x, y];
                    Tiles tile = isFloor ? Tiles.floor : Tiles.wall;
                    map[x + 1, y + 1] = tile;
                }
            }

            int middle = Mathf.FloorToInt(scale / 2);
            map[middle + 1, middle + 1] = Tiles.spawn;
            start = [middle + 1, middle + 1];

            map[RecursiveBacktracker.Blueprint.endX + 1, RecursiveBacktracker.Blueprint.endY + 1] = Tiles.relic;

            RecursiveBacktracker.DebugDraw();
        }

        internal static void SpawnMaze()
        {
            MonkeMaze = new("MonkeMaze Maze Container");
            MonkeMaze.transform.position = position;
            MonkeMaze.AddComponent<Gamemode.Manager>();
            for (int i = 0; i < scale + 2; ++i)
            {
                for (int j = 0; j < scale + 2; ++j)
                {
                    Vector3 pos = new(i, 0, j);
                    pos *= size;

                    GameObject tile = Object.Instantiate(TileSets.GetTile(map[i, j]));
                    tile.transform.parent = MonkeMaze.transform;
                    tile.transform.localPosition = pos;
                }
            }
            GameObject roof = GameObject.CreatePrimitive(PrimitiveType.Plane);
            roof.transform.parent = MonkeMaze.transform;
            roof.transform.localPosition = new Vector3(scale, 4, scale);
            roof.transform.localScale = Vector3.one * scale;
            roof.transform.eulerAngles = new Vector3(0, 0, 180);
            roof.GetComponent<MeshRenderer>().enabled = false;
            roof.GetComponent<GorillaSurfaceOverride>().overrideIndex = 59;
        }

        internal static Vector3 GetTilePos(int x, int y, bool local)
        {
            return (new Vector3(x, .5f, y) * size) + (local ? Vector3.zero : position);
        }

        internal static Dictionary<Tiles, char> tile2str = new()
        {
            { Tiles.floor , ' ' },
            { Tiles.wall  , '#' },
            { Tiles.spawn , '@' },
            { Tiles.relic , 'X' }
        };
    }
}
