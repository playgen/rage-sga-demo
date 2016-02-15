using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameController : NetworkBehaviour
{
	public static GameObject controller;
	public static GameController singleton;

	public TimerScript timerScript { get; private set; }
	[SyncVar(hook="OnTimeChange")]
	private string timer = "";
    private Text timerText;
    private string timeDecimalPoint = "0.00";

	public GameObject spaceprefab;
	private GameObject _player;
	private GameObject[] spaces = new GameObject[9];

    public bool gameInProgress = false;
    // score
    public int redCount = 0;
    public int blueCount = 0;

	private void Start()
	{
		singleton = this;
		timerScript = FindObjectOfType<TimerScript>();
        timerText = GameObject.Find("Timer").GetComponent<Text>();

		controller = gameObject;
	}

    public void StartGame()
    {
        gameInProgress = true;
        timerScript.StartTimer();
    }

	private void Update()
	{
        if (gameInProgress)
        {
            double timeLeft = timerScript.timeLeft;
            TruncateTime(timeLeft);
            if (timeLeft == 0)
            {
                gameInProgress = false;
                CheckWin();
            }
        }   
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

	public GameObject GetPlayer()
	{
		return _player;
	}

	public void SetPlayer(GameObject player)
	{
		_player = player;
	}

    [Server]
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
    }

	[Server]
	public void CheckWin()
	{
		if (redCount == 9)
		{
            SetWinner("Red");
		}
		else if (blueCount == 9)
		{
			SetWinner("Blue");
		}
        if (gameInProgress == false)
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

    private void SetWinner(string winner)
    {
        gameInProgress = false;
        timer = winner + " winner";
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
