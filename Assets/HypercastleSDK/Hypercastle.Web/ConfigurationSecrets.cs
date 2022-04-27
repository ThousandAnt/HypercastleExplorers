using System.IO;
using UnityEngine;

namespace Hypercastle.Web {
    internal class ConfigurationSecrets
    {
        public string InfuraProjectId;
        public string EtherscanToken;

        public static ConfigurationSecrets Load()
        {
            var path = Path.Combine(Application.streamingAssetsPath, "secrets.json");
            return JsonUtility.FromJson<ConfigurationSecrets>(File.ReadAllText(path));
        }
    }
}
