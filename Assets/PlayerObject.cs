using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerObject : NetworkBehaviour
{
    private GameController gameControllerScript;
	private void Start()
	{
        gameControllerScript = GameController.singleton.GetComponent<GameController>();
		if (isLocalPlayer)
		{
            gameControllerScript.SetPlayer(gameObject);
		}
	}

	public void ObjectClicked(GameObject tile)
	{
		CmdAssignState(tile);
	}

	[Command]
	public void CmdAssignState(GameObject tile)
	{
		int state;
		if (isLocalPlayer)
		{
			state = 1;
		}
		else
		{
			state = 2;
		}
        // check to see if the state is new
        if (tile.GetComponent<SpaceScript>().state != state)
        {
            RpcUpdateState(state, tile);
        }
	}

	[ClientRpc]
	private void RpcUpdateState(int newState, GameObject tile)
	{
        gameControllerScript.SetScore(tile, newState);
		tile.GetComponent<SpaceScript>().SetState(newState);
	}
}
