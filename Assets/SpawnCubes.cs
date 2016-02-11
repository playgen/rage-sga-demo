using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


public class SpawnCubes : NetworkBehaviour {

	// Use this for initialization

	void Start () {
        InstantiateCube();
	}

    void InstantiateCube()
    {
        var go = (GameObject)Instantiate(Resources.Load("Cube"), transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
    }


}
