using UnityEngine;

#if PLUGIN
using MonkeMaze.Generation;
using System.Collections.Generic;
#endif

namespace MonkeMaze.Tunnel
{
    internal class Entrance : MonoBehaviour
    {
#if PLUGIN
        internal static bool Open
        {
            get => open;
            set
            {
                Instances.ForEach(i => i.OpenEntrance(value));
                open = value;
            }
        }

        private static bool open = true;

        internal static List<Entrance> Instances = [];
        internal static GameObject Parent = new("MonkeMaze Entrance Container");

        internal Collider[] collision = [];
#endif

        public Vector3 pos = Vector3.zero;
        public Vector3 rot = Vector3.zero;
        public Vector3 scale = Vector3.zero;
        public Vector3 exitPos = Vector3.zero;

#if PLUGIN
        public void Awake()
        {
            Instances.Add(this);
            collision = GetComponentsInChildren<Collider>(true);
            OpenEntrance(Open);

            transform.parent = Parent.transform;
            transform.position = pos;
            transform.eulerAngles = rot;
            transform.localScale = scale;
        }

        public void OpenEntrance(bool value)
        {
            collision.ForEach(collider => collider.enabled = value);
            transform.Find("Closed").gameObject.SetActive(!value);
        }

        public void OnTriggerEnter(Collider collider)
        {
            if (!open || collider.name != "Body Collider") return;
            Plugin.OnMazeEnter?.Invoke();
            Rng.NewRandom();
            Creation.GenerateAll();
            Teleportation.TeleportPlayer(Creation.GetTilePos(Creation.start[0], Creation.start[1], false));
            Teleportation.ExitPos = pos + Quaternion.Euler(rot) * new Vector3(-1.5f, 1f, 0f); //exitPos;
        }
#endif
    }
}
