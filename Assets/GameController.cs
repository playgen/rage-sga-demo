using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameController : NetworkBehaviour
{

    public static GameObject controller;
    public static GameController singleton;
    public GameObject spaceprefab;
    GameObject _player;

    void Start()
    {
        singleton = this;
        controller = gameObject;
    }

    public GameObject GetPlayer()
    {
        return _player;
    }

    public void SetPlayer(GameObject player)
    {
        _player = player;
    }

    public override void OnStartServer()
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                GameObject space = (GameObject)GameObject.Instantiate(spaceprefab, transform.position, Quaternion.identity);
                space.transform.position = new Vector3(-3 + (2 * x), -3 + (2 * y), 0f);
                NetworkServer.Spawn(space);
            }
        }
    }
}