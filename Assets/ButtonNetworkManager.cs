using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ButtonNetworkManager : NetworkManager {

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	}

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        base.OnServerAddPlayer(conn, playerControllerId);
        Debug.Log("New player " + playerControllerId);
    }


}
