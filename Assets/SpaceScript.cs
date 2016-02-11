using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SpaceScript : NetworkBehaviour {
    public Color red = Color.red;
    public Color blue = Color.blue;
    public int state;
    NetworkIdentity myNetId;

    void Awake ()
    {
        myNetId = GetComponent<NetworkIdentity>();
    }

    public void setstate(int newstate)
    {
        state = newstate;
        if (state == 1) 
        {
            gameObject.GetComponent<Renderer>().material.color = red;
        }
        else
        {
            gameObject.GetComponent<Renderer>().material.color = blue;
        }
    }

    void OnMouseDown()
    {
        GameObject currentPlayer = GameController.singleton.GetComponent<GameController>().GetPlayer();
        currentPlayer.GetComponent<PlayerObject>().objectClicked(gameObject);
    }

}