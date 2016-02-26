using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimerScript : MonoBehaviour {

    private double startTime = 10.0;
    private bool startTimer = false;
    public double timeLeft;

	// Use this for initialization
	void Start () {
        timeLeft = startTime;
	}

    public void StartTimer()
    {
        startTimer = true;
    }

    public void ResetTimer()
    {
        Debug.Log("resetTimer");
        timeLeft = startTime;
        startTimer = true;
    }

	// Update is called once per frame
	void Update () {
        if (startTimer)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                timeLeft = 0;
                startTimer = false;
            }
        }
	}
}
