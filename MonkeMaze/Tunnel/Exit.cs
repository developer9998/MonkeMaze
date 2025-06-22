using UnityEngine;

#if PLUGIN
using MonkeMaze.Generation;
#endif

namespace MonkeMaze.Tunnel
{
    internal class Exit : MonoBehaviour
    {
#if PLUGIN
        public void OnTriggerEnter(Collider collider)
        {
            if (collider.name != "Body Collider") return;
            Plugin.OnMazeExit?.Invoke(false, null);
            if (NetworkSystem.Instance.InRoom) Creation.MonkeMaze.SetActive(false);
            else Creation.Remove();
            Teleportation.TeleportPlayer(Teleportation.ExitPos);
        }
#endif
    }
}
