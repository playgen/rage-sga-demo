using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class test : NetworkManager
{

	// Use this for initialization
	void Start () {
	}

    void OnPlayerConnected(NetworkPlayer player)
    {
        Debug.Log("Player connected from " + player.ipAddress + ":" + player.port);
    }

    void OnConnectedToServer()
    {
        Debug.Log("server");
    }

    // Update is called once per frame
	void Update () {
	
	}
}
