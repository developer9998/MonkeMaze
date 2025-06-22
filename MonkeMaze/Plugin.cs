using BepInEx;
using BepInEx.Logging;
using GorillaGameModes;
using MonkeMaze.Generation;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using MonkeMaze.Gamemode;
using Utilla;
using Utilla.Attributes;

namespace MonkeMaze
{
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [ModdedGamemode("monkemaze", "MONKE MAZE", GameModeType.Casual)]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public static new ManualLogSource Logger;

        public static Action OnMazeEnter;

        public static Action<bool, string> OnMazeExit;

        private GTZone[] activeZones;

        public void Awake()
        {
            Logger = base.Logger;

            HarmonyPatches.ApplyHarmonyPatches();

            Events.GameInitialized += Initialize;
            Events.RoomJoined += Joined;
            Events.RoomLeft += Left;
            OnMazeEnter += MazeEnter;
            OnMazeExit += MazeExit;

            Stats.LoadStats();
        }

        private void Initialize(object sender, EventArgs args)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MonkeMaze.Resources.maze_assets");
            var Bundle = AssetBundle.LoadFromStream(stream);

            GameObject tunnels = Bundle.LoadAsset<GameObject>("Entrances");
            tunnels.GetComponentsInChildren<Collider>().Select(collider => collider.gameObject).Distinct().ForEach(CustomMapLoader.SetupCollisions);

            GameObject[] entrances =
            [
                tunnels.transform.Find("MazeEntrance (forest)").gameObject,
                tunnels.transform.Find("MazeEntrance (cave)").gameObject,
                tunnels.transform.Find("MazeEntrance (city)").gameObject,
                tunnels.transform.Find("MazeEntrance (canyon)").gameObject,
                tunnels.transform.Find("MazeEntrance (mountain)").gameObject
            ];

            foreach (GameObject entrance in entrances) Instantiate(entrance);

            GameObject Tiles = Bundle.LoadAsset<GameObject>("Tiles");
            Tiles.GetComponentsInChildren<Collider>().Select(collider => collider.gameObject).Distinct().ForEach(CustomMapLoader.SetupCollisions);

            TileSets.Tiles = [
                Tiles.transform.Find("EmptyTile").gameObject,
                Tiles.transform.Find("WallTile").gameObject,
                Tiles.transform.Find("EntranceTile").gameObject
            ];

            GameObject SecretTiles = Bundle.LoadAsset<GameObject>("SecretTiles");
            SecretTiles.GetComponentsInChildren<Collider>().Select(collider => collider.gameObject).Distinct().ForEach(CustomMapLoader.SetupCollisions);

            TileSets.SecretTiles = [
                SecretTiles.transform.Find("TheHole").gameObject,
                SecretTiles.transform.Find("Painting").gameObject
            ];

            GameObject RelicTiles = Bundle.LoadAsset<GameObject>("RelicTiles");
            RelicTiles.GetComponentsInChildren<Collider>().Select(collider => collider.gameObject).Distinct().ForEach(CustomMapLoader.SetupCollisions);

            TileSets.RelicTiles = [
                RelicTiles.transform.Find("Monke1").gameObject,
                RelicTiles.transform.Find("Monke2").gameObject,
                RelicTiles.transform.Find("Monke3").gameObject,
                RelicTiles.transform.Find("Monke4").gameObject,
                RelicTiles.transform.Find("Stick").gameObject,
                RelicTiles.transform.Find("Snail").gameObject,
                RelicTiles.transform.Find("Banana").gameObject,
                RelicTiles.transform.Find("Bubble").gameObject,
                RelicTiles.transform.Find("Arrow").gameObject,
                RelicTiles.transform.Find("DefaultCube").gameObject
            ];

            TileSets.Compass = Bundle.LoadAsset<GameObject>("Compass");

            Bundle.Unload(false);
        }

        public void MazeEnter()
        {
            activeZones = ZoneManagement.instance.activeZones.ToArray();
            ZoneManagement.SetActiveZone(GTZone.canyon);
        }

        public void MazeExit(bool hasRelic, string relicName)
        {
            Logger.LogInfo($"MazeExit: {hasRelic} ({(hasRelic ? relicName : "None")})");

            ZoneManagement.SetActiveZones(activeZones);

            if (hasRelic)
            {
                Stats.AddScore(relicName, 1);
                Logger.LogInfo($"Scores:\n{string.Join("\n", Stats.Scores.Select(score => string.Format("{0}: {1}", score.Key, score.Value)))}");
            }
        }

        [ModdedGamemodeJoin]
        public void MazeJoin(string gamemode)
        {
            Tunnel.Entrance.Open = true;

            if (NetworkSystem.Instance is NetworkSystem netSys && netSys.IsMasterClient && netSys is NetworkSystemPUN)
            {
                var table = PhotonNetwork.CurrentRoom.CustomProperties;
                if (table.ContainsKey("CreationTime")) table["CreationTime"] = DateTimeOffset.Now.ToUnixTimeSeconds();
                else table.Add("CreationTime", DateTimeOffset.Now.ToUnixTimeSeconds());
                PhotonNetwork.CurrentRoom.SetCustomProperties(table);
            }
        }

        private void Joined(object sender, EventArgs args)
        {
            Tunnel.Entrance.Open = false;
            Creation.Remove();
        }

        private void Left(object sender, EventArgs args)
        {
            Tunnel.Entrance.Open = true;
            Creation.Remove();
        }
    }
}
