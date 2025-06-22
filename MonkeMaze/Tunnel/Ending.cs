#if PLUGIN
using MonkeMaze.Generation;
#else
using UnityEngine;
#endif

namespace MonkeMaze.Tunnel
{
#if PLUGIN
    internal class Ending : GorillaPressableButton
#else
    internal class Ending : MonoBehaviour
#endif
    {
        public string score = "";

#if PLUGIN
        public override void ButtonActivation()
        {
            base.ButtonActivation();
            Plugin.OnMazeExit?.Invoke(true, score);
            if (NetworkSystem.Instance.InRoom) Creation.MonkeMaze.SetActive(false);
            else Creation.Remove();
            Teleportation.TeleportPlayer(Teleportation.ExitPos);
        }
#endif

    }
}
