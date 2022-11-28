using System.Collections;
using System.Linq;
using Ultility;
using UnityEngine;

namespace Blockchain
{
    [RequireComponent(typeof(PythonRunner))]
    public class BlockchainRPC : Singleton<BlockchainRPC>
    {
        private IEnumerator Start()
        {
            var script = GetComponent<PythonRunner>();
            script.Run();
        }
    }
}