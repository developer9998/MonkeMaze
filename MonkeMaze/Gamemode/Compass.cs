using UnityEngine;

namespace MonkeMaze.Gamemode
{
    internal class Compass : MonoBehaviour
    {
        internal GameObject needle;
        private void Awake()
        {
            needle = transform.Find("Needle").gameObject;
        }
        private void Update()
        {
            needle.transform.localEulerAngles = Vector3.up * -transform.eulerAngles.y;
        }
    }
}
