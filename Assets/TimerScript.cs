using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimerScript : MonoBehaviour {

    private Text timeText;
    private float timeLeft;
    public float startTime;
    private bool startTimer = false;

	// Use this for initialization
	void Start () {
        timeText = GetComponent<Text>();
        timeLeft = startTime;
	}
    void OnPlayerConnected(NetworkPlayer player)
    {
        Debug.Log("Player Connected3");
    }
    public void StartTimer()
    {
        startTimer = true;
    }

	// Update is called once per frame
	void Update () {
        if (startTimer)
        {
            timeText.text = timeLeft.ToString();
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                startTimer = false;
            }
        }
           
	}
}
