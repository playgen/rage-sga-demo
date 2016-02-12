using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameController : NetworkBehaviour
{

    public static GameObject controller;
    public static GameController singleton;
    public TimerScript timerScript { get; private set; }
    public GameObject spaceprefab;
    private GameObject _player;
    private GameObject[] spaces = new GameObject[9];

    void Start()
    {
        singleton = this;
        timerScript = FindObjectOfType<TimerScript>();
        timerScript.StartTimer();
        controller = gameObject;
    }

    void OnPlayerConnected(NetworkPlayer player)
    {
        Debug.Log("Player Connected4");
    }

    void OnConnectedToServer()
    {
        Debug.Log("Connected to Server");
    }

    public GameObject GetPlayer()
    {
        return _player;
    }

    public void SetPlayer(GameObject player)
    {
        _player = player;
    }

    [Server]
    public void CheckWin()
    {
        int redCount = 0;
        int blueCount = 0;
        for (int i = 0; i < spaces.Length; i++)
        {
            int state = spaces[i].GetComponent<SpaceScript>().state;

            if (state == 1) redCount += 1;
            else if (state == 2) blueCount += 1;
        }
        if (redCount == 9) Debug.Log("Red Winner");
        else if (blueCount == 9) Debug.Log("Blue Winner");
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
                spaces[x + (y * 3)] = space;
            }
        }
    }
}