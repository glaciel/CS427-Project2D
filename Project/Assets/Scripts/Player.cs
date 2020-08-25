using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class Player : NetworkBehaviour
{
    [SyncVar]
    public int totalScore;
    [SyncVar]
    public int round1Score;
    [SyncVar]
    public int round2Score;
    [SyncVar]
    public int round3Score;
    [SyncVar]
    public int score;
    [SyncVar]
    public bool loadedScene;
    public GameObject timeScoreController;
    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        totalScore = 0;
        transform.SetParent(Prototype.NetworkLobby.MyLobbyManager.s_Singleton.transform);
        if (isLocalPlayer)
        {
            CmdFinishLoadingScene();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Command]
    public void CmdIncreaseScore()
    {
        score += 5;
    }

    [Command]

    public void CmdResetScore()
    {
        score = 0;
    }

    [Command]
    public void CmdFinishLoadingScene()
    {
        loadedScene = true;
    }

    [Command]

    public void CmdStartLoadingScene()
    {
        loadedScene = false;
    }

    [Command]

    public void CmdUpdateTotalScore()
    {
        totalScore += score;
    }

    [Command]

    public void CmdUpdateRound1Score()
    {
        round1Score = score;
    }

    [Command]

    public void CmdUpdateRound2Score()
    {
        round2Score = score;
    }

    [Command]

    public void CmdUpdateRound3Score()
    {
        round3Score = score;
    }

    [ClientRpc]

    public void RpcStartLoadingScene(int round)
    {
        if (isLocalPlayer)
        {
            CmdStartLoadingScene();
            CmdUpdateTotalScore();
            switch (round)
            {
                case 1:
                    CmdUpdateRound1Score();
                    break;
                case 2:
                    CmdUpdateRound2Score();
                    break;
                case 3:
                    CmdUpdateRound3Score();
                    break;
            }
            CmdResetScore();
        }
    }
}
