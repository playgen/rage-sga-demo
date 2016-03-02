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
            if (ServerManager.currentMatch  != null)
            {
                ServerManager.currentMatch.Quit();
            }
            GameController.singleton.ResetGame();
            NetworkManager.singleton.StopServer();
            NetworkManager.singleton.StopHost();
        }
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        if (ServerManager.currentMatch != null)
        {
            ServerManager.currentMatch.Quit();
        }
        GameController.singleton.ResetGame();
        NetworkManager.singleton.StopServer();
        NetworkManager.singleton.StopClient();
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
	{
		base.OnServerAddPlayer(conn, playerControllerId);
	}
}
