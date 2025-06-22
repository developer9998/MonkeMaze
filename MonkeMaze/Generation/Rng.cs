using Photon.Pun;
using System;
using System.Text;

namespace MonkeMaze.Generation
{
    internal static class Rng
    {
        internal static Random rand;
        internal static int RandIndex(int cap)
        {
            return rand.Next(cap);
        }
        internal static void NewRandom()
        {
            if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance is NetworkSystemPUN)
            {
                PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("CreationTime", out object time);
                string seed = NetworkSystem.Instance.RoomName + time;
                int hash = MyHash(seed);
                Plugin.Logger.LogInfo($"NewRandom (time: {time} | seed: {seed} | hash: {hash})");
                rand = new Random(hash);
                return;
            }

            rand = new Random(DateTimeOffset.Now.ToUnixTimeMilliseconds().GetHashCode());
        }

        internal static int MyHash(string str)
        {
            int hash = 0;
            foreach (byte b in Encoding.ASCII.GetBytes(str)) hash += b;
            return hash;
        }
    }
}
