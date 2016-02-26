using SocialGamification;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ButtonNetworkManager : NetworkManager
{
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (IsClientConnected())
        {
            ServerManager.currentMatch.Quit();
            GameController.singleton.ResetGame();
        }
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        if (IsClientConnected())
        {
            ServerManager.currentMatch.Quit();
            GameController.singleton.ResetGame();
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
	{
		base.OnServerAddPlayer(conn, playerControllerId);
	}
}
