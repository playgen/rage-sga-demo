using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerObject : NetworkBehaviour
{
	private void Start()
	{
		if (isLocalPlayer)
		{
			GameController.controller.GetComponent<GameController>().SetPlayer(gameObject);
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

		RpcUpdateState(state, tile);
	}

	[ClientRpc]
	private void RpcUpdateState(int newState, GameObject tile)
	{
		tile.GetComponent<SpaceScript>().SetState(newState);
	}
}
