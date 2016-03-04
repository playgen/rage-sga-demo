using SocialGamification;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerManager : NetworkBehaviour
{
	public static int isSearching = 0;

	public static Match currentMatch { get; set; }
    public static string currentActivityId { get; set; }
    public static string currentRoleId { get; set; }
    public static Activity currentActivity { get; set; }
    public static Role currentRole { get; set; }
    public static List<Goal> currentGoals { get; set; }
    public static List<ActorGoal> currentActorGoals { get; set; }
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
        currentActivityId = "436958ff-e160-11e5-9ea4-6057185dd7ce";
        currentRoleId = "cfdf02f7-e160-11e5-9ea4-6057185dd7ce";
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
        Match.LoadOngoing(idTournament, true, string.Empty, (Match[] matches) =>
        {
            if (matches == null || matches.Length == 0)
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
        UpdateActivity();
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
                    Match.LoadOngoing("0", true, string.Empty, (Match[] matches) =>
                    {
                        if (matches == null || matches.Length == 0)
                        {
                            Debug.Log("No match");
                        }
                        else
                        {
                            foreach (Match m in matches)
                            {
                                if (!m.finished)
                                {
                                    ServerManager.currentMatch = m;
                                }
                            }
                        }
                    });
					Debug.Log("Made match: " + match);
				}
			});
        }
	}

    public void RestartMatch(Action<Match> callback)
    {
        Debug.Log("RestartMatch");
        Match currentMatchClone = new Match(currentMatch);
        currentMatchClone.Duplicate((Match match, bool success, string error) =>
        {
            if (!success)
            {
                Debug.Log("Restart Match. Error: " + error);
            }
            callback(match);
        });
    }

    public void EndMatch(int score)
    {
        if (ServerManager.currentMatch.finished)
        {
            checkingScore = true;
        }
        else {
            if (ServerManager.currentMatch.users.Count > 0)
            {
                currentMatch.Score((float)score, (bool success, string err) =>
                {
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
            } else
            {
                Debug.Log("No players found");
                ServerManager.currentMatch.Quit();
            }
        }
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

    public static void UpdateActivity()
    {
        currentActivity = null;
        currentRole = null;
        currentGoals = new List<Goal>();
        currentActorGoals = new List<ActorGoal>();
        Activity.GetActivity(currentActivityId, (Activity activity) =>
        {
            if (activity == null)
            {
                currentActivity = null;
                Debug.Log("No activity");
            }
            else
            {
                currentActivity = activity;
                Debug.Log(activity.name);
                Role.GetRole(currentRoleId, (Role role) =>
                {
                    if (role == null)
                    {
                        currentRole = null;
                        Debug.Log("No role");
                    }
                    else
                    {
                        currentRole = role;
                        Debug.Log(role.name);
                        Goal.GetActivityGoals(currentActivity.id, (List<Goal> goalList) =>
                        {
                            if (goalList == null || goalList.Count == 0)
                            {
                                Debug.Log("No goals");
                            }
                            else
                            {
                                currentGoals = goalList;
                                currentActorGoals = new List<ActorGoal>();
                                foreach (Goal goal in currentGoals)
                                {
                                    ActorGoal.CreateActorGoal(goal.id, goal.concernId, goal.rewardResourceId, currentActivity.id, currentRole.id, (ActorGoal g) =>
                                    {
                                        if (g == null)
                                        {
                                            Debug.Log("Error creating actor goal");
                                        }
                                        else
                                        {
                                            Debug.Log("Goal created");
                                            currentActorGoals.Add(g);
                                        }
                                    });
                                }
                            }
                        });
                    }
                });
            }
        });
    }
}
