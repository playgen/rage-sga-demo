using SocialGamification;
using System;
using System.Collections.Generic;
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
        GameController.tracker = Tracker.T();
        string username;
        string password;
        if (isServer)
        {
            username = "mayur";
            password = "mayur";
            GameController.tracker.Screen("server_start");
        }
        else
        {
            username = "matt";
            password = "matt";
            GameController.tracker.Screen("client_start");
        }
        GameController.tracker.RequestFlush();
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
                    currentRole = "Red";
                    UpdateGoals();
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
                            currentRole = "Blue";
                            UpdateGoals();
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
            }
            else
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

    public static void UpdateGoals()
    {
        Role.GetRole(currentRole, (Role role) =>
        {
            if (role == null)
            {
                Debug.Log("No role");
            }
            else
            {
                Debug.Log(role.name);
                Goal.GetRoleGoals(role.id, (List<Goal> goalList) =>
                {
                    if (goalList == null || goalList.Count == 0)
                    {
                        Debug.Log("No goals");
                    }
                    else
                    {
                        foreach (Goal goal in goalList)
                        {
                            ActorGoal.CreateActorGoal(goal.id, goal.concernId, goal.rewardResourceId, role.activityId, role.id, (ActorGoal g) =>
                            {
                                if (g == null)
                                {
                                    Debug.Log("Error creating actor goal");
                                }
                                else
                                {
                                    Debug.Log("Goal created");
                                }
                            });
                        }
                    }
                });
            }
        });
    }
}
