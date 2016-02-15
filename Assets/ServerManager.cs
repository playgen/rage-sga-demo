using SocialGamification;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ServerManager : NetworkBehaviour
{
	public static int IsSearching = 0;

	public static Match CurrentMatch { get; set; }

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

	public static void SetIsSearching(int val)
	{
		SocialGamificationManager.localUser.customData["isSearching"] = val;
		SocialGamificationManager.localUser.Update((bool success, string error) =>
		{
			if (success)
			{
				IsSearching = val;
			}
			else
			{
				SocialGamificationManager.localUser.customData["isSearching"] = IsSearching;
			}
		});
	}

	private void JoinMatch()
	{
		if (!isServer)
		{
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
					CurrentMatch = match;
					Debug.Log("Made match: " + match);
				}
			});
		}
	}

	// Update is called once per frame
	private void Update()
	{
	}
}
