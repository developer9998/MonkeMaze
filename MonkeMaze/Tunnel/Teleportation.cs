using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;

namespace MonkeMaze.Tunnel
{
    internal static class Teleportation
    {
        internal static Vector3 ExitPos = Vector3.zero;

        internal static void TeleportPlayer(Vector3 target)
        {
            GTPlayer __instance = GTPlayer.Instance;
            TeleportPlayer(target, __instance.transform.eulerAngles.y);
        }

        internal static void TeleportPlayer(Vector3 target, float angle) // Teleport function! Yay!
        {
            GTPlayer __instance = GTPlayer.Instance;
            Vector3 correctedPosition = target - __instance.bodyCollider.transform.position + __instance.transform.position;
            __instance.TeleportTo(correctedPosition, Quaternion.Euler(__instance.transform.eulerAngles.WithY(angle)), true);
        }
    }

    // Thank you Auralius
}
