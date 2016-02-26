using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using SocialGamification;

public class GameController : NetworkBehaviour
{
	public static GameObject controller;
	public static GameController singleton;
    private ServerManager serverManager;

    //UI
	public TimerScript timerScript { get; private set; }
	[SyncVar(hook="OnTimeChange")]
	private string timer = "";
    private Text timerText;
    private string timeDecimalPoint = "0.00";
    private Button resetBtn;

	public GameObject spaceprefab;
	private GameObject _player;
	private GameObject[] spaces = new GameObject[9];

    [SyncVar]
    public bool gameInProgress = false;
    // score
    [SyncVar]
    public int redCount = 0;
    [SyncVar]
    public int blueCount = 0;

	private void Start()
	{
        controller = gameObject;
        singleton = this;
		timerScript = FindObjectOfType<TimerScript>();
        timerText = GameObject.Find("Timer").GetComponent<Text>();
        resetBtn = GameObject.Find("Reset").GetComponent<Button>();
        serverManager = GameObject.Find("ServerManager").GetComponent<ServerManager>();
	}

    public void StartGame()
    {
        gameInProgress = true;
        timerScript.ResetTimer();
    }

    public void ToggleBtn()
    {
        if (resetBtn.interactable)
        {
            resetBtn.interactable = false;
        }
        else
        {
            resetBtn.interactable = true;
        }
    }

    public void ResetGame()
    {
        Debug.Log("Reset Game. Server: " + isServer);
 
        //ToggleBtn();
        //var objects = GameObject.FindObjectsOfType<SpaceScript>();
        //foreach (var obj in objects)
        //{
        //    Destroy(obj.gameObject);
        //}
        //blueCount = 0;
        //redCount = 0;
        //OnStartServer();
        //serverManager.RestartMatch((Match match) => 
        //{
        //    if (match != null)
        //    {
        //        ServerManager.currentMatch = match; // put this back in servermanager if works
        //        RpcClientJoinGame();
        //    }
        //});
       
    }

    [ClientRpc]
    private void RpcClientJoinGame()
    {
        //serverManager.SearchMatch();
        ServerManager.SetIsSearching(1);
    }

    public void OnResetClick()
    {
        GetPlayer().GetComponent<PlayerObject>().CmdResetGame();
    }

	private void Update()
	{
        if (gameInProgress)
        {
            double timeLeft = timerScript.timeLeft;
            TruncateTime(timeLeft);
            if (timeLeft <= 0)
            {
                Debug.Log("TimeUp");
                gameInProgress = false;
                //CheckWin();
                RpcClientCheckWin();
            }
        }   
	}

    [ClientRpc]
    private void RpcClientCheckWin()
    {
        //Force Game End on Client
        CheckWin(true);
    }

    public void SetScore(GameObject tile, int player)
    {
        int state = tile.GetComponent<SpaceScript>().state;
        if (player == 1)
        {
            if (state == 2)
            {
                blueCount--;
            }
            redCount++;
        }
        else
        {
            if (state == 1)
            {
                redCount--;
            }
            blueCount++;
        }
        CheckWin();
    }

	public void CheckWin(bool forceGameEnd = false)
	{
        bool gameEnded;
        if (forceGameEnd)
        {
            gameEnded = true;
        }
        else
        {
            gameEnded = !gameInProgress;
        }

        bool scoreSent = false;
        if (redCount == 9)
		{
            SetWinner("Red");
            scoreSent = true;
		}
		else if (blueCount == 9)
		{
           
			SetWinner("Blue");
            scoreSent = true;
		}
        
        if (!scoreSent && gameEnded)
        {
            if (redCount > blueCount)
            {
                SetWinner("Red");
            }
            else if (blueCount > redCount) 
            {
                SetWinner("Blue");
            }
            else 
            {
                SetWinner("No");
            }
        }
	}

    public void SetWinner(string winner)
    {
        gameInProgress = false;

        Debug.Log("Setwinner: " + isServer);
        int score;
        if (isServer) 
        {
            score = redCount;
        }
        else
        {
            score = blueCount;
        }
        // SGA game end
        serverManager.EndMatch(score);

        timer = winner + " winner";
    }

    private void TruncateTime(double timeLeft)
    {
        string truncateTime = timeLeft.ToString(timeDecimalPoint);
        if (timer != truncateTime)
        {
            timer = truncateTime;
        }
    }

    private void OnTimeChange(string time)
    {
        timerText.text = time;
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

    public GameObject GetPlayer()
    {
        return _player;
    }

    public void SetPlayer(GameObject player)
    {
        _player = player;
    }

}
