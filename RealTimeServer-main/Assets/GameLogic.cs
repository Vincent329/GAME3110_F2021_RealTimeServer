using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{

    LinkedList<int> connectedClients;

    float durationUntilNextBalloon;

    int lastRefID;

    LinkedList<int> activeBalloonIDs;

    LinkedList<ScoreAndConnectionID> scoreAndConnectionIDs;

    void Start()
    {
        NetworkedServerProcessing.SetGameLogic(this);
        connectedClients = new LinkedList<int>();
        activeBalloonIDs = new LinkedList<int>();
        scoreAndConnectionIDs = new LinkedList<ScoreAndConnectionID>();
    }

    void Update()
    {
        //Debug.Log("count == " + scoreAndConnectionIDs.Count);

        if (connectedClients.Count > 0)
        {
            durationUntilNextBalloon -= Time.deltaTime;

            if (durationUntilNextBalloon < 0)
            {
                durationUntilNextBalloon = 1f;

                float screenPositionXPercent = Random.Range(0.0f, 1.0f);
                float screenPositionYPercent = Random.Range(0.0f, 1.0f);

                lastRefID++;
                string msgToSendToClients = ServerToClientSignifiers.SpawnBalloon + "," + screenPositionXPercent + "," + screenPositionYPercent + "," + lastRefID;


                foreach (int cid in connectedClients)
                    NetworkedServerProcessing.SendMessageToClient(msgToSendToClients, cid);

                activeBalloonIDs.AddLast(lastRefID);

                //we dont need this!!!
                //Vector2 screenPosition = new Vector2(screenPositionXPercent * (float)Screen.width, screenPositionYPercent * (float)Screen.height);
                //SpawnNewBalloon(screenPosition);
            }

        }
    }

    public void AddConnectedClient(int clientID)
    {
        foreach (ScoreAndConnectionID sac in scoreAndConnectionIDs)
        {
            string msg = ServerToClientSignifiers.ScoreAndID + "," + 0 + "," + clientID;
            NetworkedServerProcessing.SendMessageToClient(msg, sac.connectionID);
        }

        connectedClients.AddLast(clientID);
        scoreAndConnectionIDs.AddLast(new ScoreAndConnectionID(0, clientID));

        foreach (ScoreAndConnectionID sac in scoreAndConnectionIDs)
        {
            string msg = ServerToClientSignifiers.ScoreAndID + "," + sac.score + "," + sac.connectionID;
            NetworkedServerProcessing.SendMessageToClient(msg, clientID);
        }

    }

    public void RemoveConnectedClient(int clientID)
    {
        connectedClients.Remove(clientID);
        ScoreAndConnectionID sac = GetScoreAndConnectionIDWithClientID(clientID);
        scoreAndConnectionIDs.Remove(sac);

        foreach (int cid in connectedClients)
        {
            string msg = ServerToClientSignifiers.PlayerHasLeft + "," + clientID;
            NetworkedServerProcessing.SendMessageToClient(msg, cid);
        }
    }

    public void PopBalloon(int balloonID, int playerID)
    {
        if (activeBalloonIDs.Contains(balloonID))
        {
            //we have a hit!!!
            string msgToSend = ServerToClientSignifiers.BalloonWasPopped + "," + balloonID + "," + playerID;
            foreach (int cid in connectedClients)
                NetworkedServerProcessing.SendMessageToClient(msgToSend, cid);

            GetScoreAndConnectionIDWithClientID(playerID).score++;
        }
        //else
        //we need to let the client know that they did not hit the balloon first

        //PrintOutScore();

    }

    private ScoreAndConnectionID GetScoreAndConnectionIDWithClientID(int clientID)
    {
        foreach (ScoreAndConnectionID sac in scoreAndConnectionIDs)
        {
            if (clientID == sac.connectionID)
                return sac;
        }

        return null;
    }

    private void PrintOutScore()
    {
        foreach (ScoreAndConnectionID sac in scoreAndConnectionIDs)
        {
            Debug.Log("#" + sac.connectionID + " : " + sac.score);
        }
    }

}


public class ScoreAndConnectionID
{
    public int score;
    public int connectionID;

    public ScoreAndConnectionID(int Score, int ConnectionID)
    {
        score = Score;
        connectionID = ConnectionID;
    }
}


///TASK LIST--------
//UI
//track score on server!
//create contain class with clientConnectID and score
//
//
//
//
//
//What happens when a player connects?
//player is sent a series of msgs from the server: foreach connectclient registered within scoring system: 
//ServerToClientSignifiers.PlayerAndScore + "," + playerID + "," + playerScore;
//
//
//What happens to already connected players when another player connects?
//ServerToClientSignifiers.PlayerHasJoined + "," + playerID
//
//
//Add playerID who popped balloon to end of BalloonWasPopped signifier msg
//
//
//----
//
//
//What happens when a player DC?
//ServerToClientSignifiers.PlayerHasLeft + "," + playerID
//
//
//

//
//
//Sort order of score contain things
//
//


#region old notes

//Task List
//
//check if clients have joined?
//
//Server must define the location of each balloon
//Server must send location of new balloon spawn to client
//Client must spawn the balloon, after recieving spawn signifier
//
//Generate a Unique Reference ID
//Send unique ref id to client when we send a spawn balloon signifier
//on balloon click, send to server the unique ref id that has been clicked
//
//
//
//
//
//on clientBalloonClickSignifer recieved by server, 
//check to see if balloon reference ID is in list of active balloons
//if...
//else...
//
//
//
//
//
//

#endregion