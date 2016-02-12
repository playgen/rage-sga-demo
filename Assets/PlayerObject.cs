using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerObject : NetworkBehaviour
{

    void Start()
    {
        if (isLocalPlayer)
        {
            GameController.controller.GetComponent<GameController>().SetPlayer(gameObject);
        }
    }

    void OnPlayerConnected(NetworkPlayer player)
    {
        Debug.Log("Player Connected");
    }

    void OnConnectedToServer()
    {
        Debug.Log("Connected to Server2");
    }
    public void ObjectClicked(GameObject tile)
    {
        CmdAssignState(tile);
    }

    [Command]
    public void CmdAssignState(GameObject tile)
    {
        int state;

        if (isLocalPlayer) state = 1;
        else state = 2;

        RpcUpdateState(state, tile);
    }

    [ClientRpc]
    void RpcUpdateState(int newState, GameObject tile)
    {
        tile.GetComponent<SpaceScript>().SetState(newState);

    }

}