using UnityEngine;

namespace MonkeMaze.Generation
{
    internal static class TileSets
    {
        internal static GameObject[] Tiles;
        internal static GameObject[] SecretTiles;
        internal static GameObject[] RelicTiles;
        internal static GameObject Compass;

        internal static GameObject GetTile(Tiles tile)
        {
            int i = (int)tile;
            if (i < Tiles.Length && i >= 0) return Tiles[i];
            if (tile == Generation.Tiles.relic)
            {
                GameObject relicTile = null;

                if (NetworkSystem.Instance.InRoom)
                {
                    switch (Rng.MyHash(NetworkSystem.Instance.RoomName))
                    {
                        case 829:
                            relicTile = RelicTiles[5];
                            break;
                        case 575:
                            relicTile = RelicTiles[8];
                            break;
                        case 813:
                            relicTile = RelicTiles[6];
                            break;
                        case 621:
                            relicTile = RelicTiles[4];
                            break;
                        case 995:
                            relicTile = SecretTiles[0];
                            break;
                        case 921:
                            relicTile = RelicTiles[7];
                            break;
                        case 49:
                            relicTile = RelicTiles[9];
                            break;
                        case 681:
                            relicTile = SecretTiles[1];
                            break;
                        case 623:
                            relicTile = Tiles[0];
                            break;
                        case 692:
                            relicTile = Tiles[2];
                            break;
                    }

                    if (relicTile)
                    {
                        Plugin.Logger.LogInfo($"Selected hash-based relic tile \"{relicTile.name}\"");
                        return relicTile;
                    }
                }

                relicTile = RelicTiles[Rng.RandIndex(RelicTiles.Length)];
                Plugin.Logger.LogInfo($"Selected random relic tile \"{relicTile.name}\"");

                return relicTile;
            }

            return Tiles[0];
        }
    }
}
