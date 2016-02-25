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
            else
            {
                JoinMatch();
            }
		});
	}

    void Update()
    {
        if (lastRequestTime.Year == 1337 || lastRequestTime.AddSeconds(timeInterval) < DateTime.Now)
        {
            lastRequestTime = DateTime.Now;

            if (isSearching == 1 && !GameController.singleton.gameInProgress)
            {
                Debug.Log("Checking Matches");
                SearchMatch();
            }

            if (checkingScore)
            {
                CheckOpponentScore();
            }
        }
    }

    public void SearchMatch(string idTournament = "0")
    {
        Match.Load(idTournament, true, string.Empty, (Match[] matches) =>
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
                        SetIsSearching(0, true, isServer);
                        return;
                    }
                }
            }
        });
    }

	public static void SetIsSearching(int val, bool startGame = false, bool server = true)
	{
        var oldVal = isSearching;
        isSearching = val;
		SocialGamificationManager.localUser.customData["isSearching"] = val;
		SocialGamificationManager.localUser.Update((bool success, string error) =>
		{
			if (success)
			{
				isSearching = val;
                if (startGame)
                {
                    if (server)
                    {
                        Debug.Log("match found and im the server");
                        GameController.controller.GetComponent<GameController>().StartGame();
                    }
                    else
                    {
                        Debug.Log("match found and im the client");
                        GameController.singleton.GetPlayer().GetComponent<PlayerObject>().CmdStartGame();
                    }
                }
			}
			else
			{
                Debug.Log("Success: " + success + ". Error: " + error + ". SetIsSearching");
                isSearching = oldVal;
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
					ServerManager.currentMatch = match;
                    // DO STUFF HERE
					Debug.Log("Made match: " + match);
				}
			});
		}
	}

    public void RestartMatch(Action<Match> callback)
    {
        Debug.Log("RestartMatch");
        Match currentMatchClone = new Match(currentMatch);
        currentMatchClone.Duplicate((Match match) =>
        {
            callback(match);
        });
    }

    public void EndMatch(int score)
    {
        currentMatch.Score((float)score, (bool success, string err) => {
            Debug.Log("Success: " + success + ". Error: " + err + ". EndMatch");
            if (success)
            {
                checkingScore = true;       
            }
            else
            {
                Debug.Log("Trying to .score again");
                EndMatch(score);                // this may fix our problem
            }
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
                        Debug.Log("Found opponent score: " + opponentScore + ". Server: " + isServer);
                        checkingScore = false;
                        currentMatch.End();
                        GameController.controller.GetComponent<GameController>().ToggleBtn();
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
