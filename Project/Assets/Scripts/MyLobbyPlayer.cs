using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Prototype.NetworkLobby
{
    [System.Serializable]
    public class MyLobbyPlayer : NetworkLobbyPlayer
    {
        [SyncVar]
        public string rating;
        [SyncVar]
        public string rank;
        [SyncVar]
        public string nickname;
        [SyncVar]
        public string fishName;
        [SyncVar]
        public int round1Score;
        [SyncVar]
        public int round2Score;
        [SyncVar]
        public int round3Score;
        [SyncVar]
        public int totalScore;
        [SyncVar]
        public bool loadResultScene = false;
        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(this);
            if (isLocalPlayer)
            {
                SendReadyToBeginMessage();
                CmdUpdateDataToServer(MainPlayer.instance.playerData.rating.ToString(),
                                      MainPlayer.instance.playerData.rank.ToString(),
                                      MainPlayer.instance.playerData.nickname,
                                      MainPlayer.instance.fishName);
                MyLobbyManager.s_Singleton.currentRound = 1;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        //This is a hook that is invoked on all player objects when entering the lobby.
        public override void OnClientEnterLobby()
        {
            base.OnClientEnterLobby();
        }

        [Command]
        public void CmdDoneLoadResultScene()
        {
            loadResultScene = true;
        }

        [Command]
        public void CmdUpdateDataToServer(string rating, string rank, string nickname, string fishName)
        {
            this.rating = rating;
            this.rank = "Rank: " + rank;
            this.nickname = nickname;
            this.fishName = fishName;
        }

        public void UpdateScore(int round1, int round2, int round3, int total)
        {
            round1Score = round1;
            round2Score = round2;
            round3Score = round3;
            totalScore = total;
        }

        [ClientRpc]
        public void RpcUpdateCountdown(int countdown)
        {
            MyLobbyManager.s_Singleton.countdownPanel.gameObject.SetActive(countdown != 0);
        }

        [ClientRpc]
        public void RpcDeactiveInfoPanel()
        {
            MyLobbyManager.s_Singleton.infoPanel.gameObject.SetActive(false);
        }

        [ClientRpc]

        public void RpcShowInformation()
        {
            ShowInformation();
        }

        [Server]

        public void UpdateDataFromServer()
        {
            if (hasAuthority)
            {
                foreach (Player p in MyLobbyManager.s_Singleton.players)
                {
                    if (p.isLocalPlayer)
                    {
                        UpdateScore(p.round1Score, p.round2Score, p.round3Score, p.totalScore);
                        break;
                    }
                }
            }
            else
            {
                foreach (Player p in MyLobbyManager.s_Singleton.players)
                {
                    if (!p.isLocalPlayer)
                    {
                        UpdateScore(p.round1Score, p.round2Score, p.round3Score, p.totalScore);
                        break;
                    }
                }
            }
        }
        public void ShowInformation()
        {
            if (isLocalPlayer)
            {
                GameObject canvas = GameObject.Find("Canvas");
                if (MyLobbyManager.s_Singleton.isInGame)
                {
                    Animator anim = canvas.GetComponent<Animator>();
                    anim.SetTrigger("LobbyToGame");
                }
                ArenaInfo local = GameObject.Find("Main User").GetComponent<ArenaInfo>();
                ArenaInfo opp = GameObject.Find("Enemy").GetComponent<ArenaInfo>();
                for (int i = 0; i < MyLobbyManager.s_Singleton.lobbySlots.Length; ++i)
                {
                    MyLobbyPlayer cur = MyLobbyManager.s_Singleton.lobbySlots[i] as MyLobbyPlayer;
                    if (cur.isLocalPlayer)
                    {
                        local.rank.text = cur.rank;
                        local.rating.text = cur.rating;
                        local.nickname.text = MyLobbyManager.s_Singleton.localNick = cur.nickname; 
                    }
                    else
                    {
                        FishVSContainer fishVSContainer = FindObjectOfType<FishVSContainer>();
                        fishVSContainer.instantiateFish(cur.fishName);
                        opp.rank.text = cur.rank;
                        opp.rating.text = cur.rating;
                        opp.nickname.text = MyLobbyManager.s_Singleton.oppNick = cur.nickname;
                    }
                }
            }
        }
    }
}
