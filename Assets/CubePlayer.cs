using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class CubePlayer : NetworkBehaviour {

    struct CubeState
    {
        public Color color;
    }

    [SyncVar] CubeState state;

	// Use this for initialization


    void Awake()
    {
        NetworkIdentity n = gameObject.GetComponent<NetworkIdentity>();

        n.localPlayerAuthority = true;

    }

	void Start () {


        InitState();
	}

    [Server] 
    void InitState()
    {
        state = new CubeState
        {
            color = Color.green
        };
    }
    
    void OnStartLocalPlayer()
    {
        Debug.Log("Start Local Player");
    }

    void OnMouseDown()
    {
        Debug.Log(isServer);
        CmdSetColourOnServer(isServer);
    }

    void Update()
    {
        SyncState();
    }

    [Command]
    void CmdSetColourOnServer(bool serverColour)
    {
        state = SetCubeColour(serverColour);
    }

    CubeState SetCubeColour(bool blueColour)
    {
        Color newColour;
        if (blueColour) newColour = Color.blue;
        else newColour = Color.red;

        return new CubeState
        {
            color = newColour
        };
    }


    void SyncState()
    {
        gameObject.GetComponent<Renderer>().material.color = state.color;
    }

    //[Command]
    //void CmdMoveOnServer(KeyCode arrowKey)
    //{
    //    state = Move(state, arrowKey);
    //}

    //CubeState Move(CubeState previous, KeyCode arrowKey)
    //{
    //    int dx = 0;
    //    int dy = 0;
    //    switch (arrowKey)
    //    {
    //        case KeyCode.UpArrow:
    //            dy = 1;
    //            break;
    //        case KeyCode.DownArrow:
    //            dy = -1;
    //            break;
    //        case KeyCode.RightArrow:
    //            dx = 1;
    //            break;
    //        case KeyCode.LeftArrow:
    //            dx = -1;
    //            break;
    //    }
    //    return new CubeState
    //    {
    //        x = dx + previous.x,
    //        y = dy + previous.y
    //    };
    //}

}

