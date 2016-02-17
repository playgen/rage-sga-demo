using SocialGamification;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ButtonNetworkManager : NetworkManager
{
	// Use this for initialization
	private void Start()
	{
	}

	// Update is called once per frame
	private void Update()
	{
	}

	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
	{
		base.OnServerAddPlayer(conn, playerControllerId);
        Debug.Log("NumPlayers: " + numPlayers);
		if (numPlayers == 2)
		{
			Match.Load("0", true, string.Empty, (Match[] matches) =>
			{
				if (matches.Length == 0)
				{
					Debug.Log("No match");
				}
				else
				{
					foreach (Match match in matches)
					{
						if (!match.finished)
						{
							Debug.Log("Found match with " + match.users);
							ServerManager.currentMatch = match;
                            //Start Game
                            GameController.controller.GetComponent<GameController>().StartGame();
							return;
						}
					}
				}
			});
		}
	}
}
