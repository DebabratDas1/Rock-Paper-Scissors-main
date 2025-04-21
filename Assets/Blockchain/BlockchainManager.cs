using System.Collections.Generic;
using UnityEngine;


namespace DD.Web3
{
    public class BlockchainManager : MonoBehaviour
    {
        [SerializeField] private BuildNetworkSO buildNetworkSO;
        [SerializeField] private List<NetworkConfigSO> networkConfigs;

        public NetworkConfigSO currentConfig;

        private void Awake()
        {
            currentConfig = networkConfigs.Find(config => config.network == buildNetworkSO.selectedNetwork);
            if (currentConfig == null)
            {
                Debug.LogError("No matching network config found!");
            }
            else
            {
                Debug.Log("Current Selected Network is " + currentConfig.name);
            }
        }
    }
}

