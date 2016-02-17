using SocialGamification;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ServerManager : NetworkBehaviour
{
	public static int isSearching = 0;
	public static Match currentMatch { get; set; }

    private bool checkingScore = false;
    private double timeInterval = 3f;
    private DateTime lastRequestTime = new DateTime(1337, 1, 1);

	// Use this for initialization
	private void Start()
	{
		string username = "matt";
		string password = "matt";

		if (isServer)
		{
			username = "mayur";
			password = "mayur";
		}
		SocialGamificationManager.platform.Authenticate<User>(username, password, (bool success, string error) =>
		{
			if (isServer)
			{
				SetIsSearching(1);
			}
			JoinMatch();
		});
	}

    void Update()
    {
        if (checkingScore) 
        {
            if (lastRequestTime.Year == 1337 || lastRequestTime.AddSeconds(timeInterval) < DateTime.Now)
            {
                lastRequestTime = DateTime.Now;
                Debug.Log("Checking Matches");

                CheckOpponentScore();
            }
        }
    }

	public static void SetIsSearching(int val)
	{
		SocialGamificationManager.localUser.customData["isSearching"] = val;
		SocialGamificationManager.localUser.Update((bool success, string error) =>
		{
			if (success)
			{
				isSearching = val;
			}
			else
			{
				SocialGamificationManager.localUser.customData["isSearching"] = isSearching;
			}
		});
	}

	private void JoinMatch()
	{
		if (!isServer)
		{
            Debug.Log("Client Join Match");
			SearchCustomData[] searchData = {
			new SearchCustomData("isSearching", eSearchOperator.Equals, "1")
		};

			Match.QuickMatch(false, searchData, 1, (Match match) =>
			{
				if (match == null)
				{
					Debug.Log("No Match");
				}
				else
				{
					currentMatch = match;
					Debug.Log("Made match: " + match);
				}
			});
		}
	}

    public void EndMatch(int score)
    {
        currentMatch.Score((float)score, (bool success, string err) => {
            Debug.Log("Success: " + success + ". Error: " + err);
            checkingScore = true;
        });
    }

    private void CheckOpponentScore()
    {
        Match match = currentMatch;

        if (match != null)
        {
            match.GetScore((bool success, float opponentScore, float ownScore, string error) =>
            {
                if (success)
                {
                    if (opponentScore != null)
                    {
                        checkingScore = false;
                        currentMatch.End();
                    }
                    else
                    {
                        Debug.Log("Opponent has not updated score yet.");
                    }
                }
                else
                {
                    Debug.Log("Couldn't get score: " + error);
                }
            });
        }
        else
        {
            Debug.Log("NO CURRENT MATCH");
        }
    }
}
