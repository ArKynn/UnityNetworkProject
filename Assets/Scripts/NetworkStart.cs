using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkStart : MonoBehaviour
{
    private NetworkManager _networkManager;
    void Start()
    {
        _networkManager = GetComponent<NetworkManager>();
        
        string[] args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "server")
            {
                _networkManager.StartServer();
            }
            else if (args[i] == "client")
            {
                _networkManager.StartClient();
            }
        }
    }
}
