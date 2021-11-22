using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class NetworkedServerProcessing
{
    #region Send and Receive Data Functions
    static public void ReceivedMessageFromClient(string msg, int id)
    {
        Debug.Log("msg received = " + msg + ".  connection id = " + id);

        string[] csv = msg.Split(',');
        int signifier = int.Parse(csv[0]);

        if (signifier == ClientToServerSignifiers.BalloonClicked)
        {
            int refID = int.Parse(csv[1]);
            Debug.Log("Balloon with ref ID == " + refID + " has been clicked!");
            gameLogic.GetComponent<GameLogic>().PopBalloon(refID, id);
        }
        // else if (signifier == ClientToServerSignifiers.asd)
        // {

        // }

        //gameLogic.DoSomething();
    }
    static public void SendMessageToClient(string msg, int id)
    {
        networkedServer.SendMessageToClient(msg, id);
    }

    #endregion

    #region Connection Events
    static public void DisconnectionEvent(int clientConnectionID)
    {
        Debug.Log("New Disconnection, ID == " + clientConnectionID);
        gameLogic.GetComponent<GameLogic>().RemoveConnectedClient(clientConnectionID);
    }
    static public void ConnectionEvent(int clientConnectionID)
    {
        Debug.Log("New Connection, ID == " + clientConnectionID);
        gameLogic.GetComponent<GameLogic>().AddConnectedClient(clientConnectionID);
    }

    #endregion

    #region Setup
    static NetworkedServer networkedServer;
    static GameLogic gameLogic;

    static public void SetNetworkedServer(NetworkedServer NetworkedServer)
    {
        networkedServer = NetworkedServer;
    }
    static public NetworkedServer GetNetworkedServer()
    {
        return networkedServer;
    }
    static public void SetGameLogic(GameLogic GameLogic)
    {
        gameLogic = GameLogic;
    }

    #endregion
}

#region Protocol Signifiers
static public class ClientToServerSignifiers
{
    public const int BalloonClicked = 1;
}

static public class ServerToClientSignifiers
{
    public const int SpawnBalloon = 1;
    public const int BalloonWasPopped = 2;
    public const int ScoreAndID = 3;
    public const int PlayerHasLeft = 4;
}

#endregion

