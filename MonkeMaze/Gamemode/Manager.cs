using GorillaLocomotion;
using MonkeMaze.Generation;
using UnityEngine;

namespace MonkeMaze.Gamemode
{
    internal class Manager : MonoBehaviour
    {
        internal GameObject compass;
        internal Vector3 lastComputerPos = Vector3.zero;
        internal Vector3 lastComputerRot = Vector3.zero;

        private void OnEnable()
        {
            compass = Instantiate(TileSets.Compass, GTPlayer.Instance.rightControllerTransform);
            compass.AddComponent<Compass>();
            compass.transform.localEulerAngles = new Vector3(0, 0, -90);
            compass.transform.localScale = Vector3.one * .05f;
            compass.transform.localPosition = new Vector3(.05f, 0, 0) + GTPlayer.Instance.rightHandOffset;
        }
        private void OnDisable()
        {
            Destroy(compass);
        }
    }
}
