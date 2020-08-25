using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using UnityEngine.SceneManagement;

namespace Prototype.NetworkLobby
{
    public class MyLobbyManager : NetworkLobbyManager
    {
        static public MyLobbyManager s_Singleton;
        protected bool _disconnectServer = false;
        protected ulong _currentMatchID;
        public LobbyInfoPanel infoPanel;
        public LobbyCountdownPanel countdownPanel;
        public LobbyInfoPanel disconnectPanel;
        public List<Player> players;
        string[] gameToPlay;
        public int currentRound;
        public bool isInGame = false;
        NodeID _currentNodeID;
        //nickname of 2 players
        public string localNick;
        public string oppNick;
        // Start is called before the first frame update
        void Start()
        {
            gameToPlay = new string[3];
            s_Singleton = this;
            DontDestroyOnLoad(gameObject);
            players = new List<Player>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void OnLobbyClientSceneChanged(NetworkConnection conn)
        {
            if (SceneManager.GetSceneAt(0).name != lobbyScene)
            {
                infoPanel.gameObject.SetActive(false);
                Player[] players = GetComponentsInChildren<Player>();
                foreach (Player p in players)
                {
                    if(p.isLocalPlayer)
                    {
                        p.CmdFinishLoadingScene();
                    }
                }
            }
            //Back to Main Menu scene
            else
            {
                isInGame = false;
                for (int i = 0; i < lobbySlots.Length; ++i)
                {
                    if (lobbySlots[i].isLocalPlayer)
                    {
                        (lobbySlots[i] as MyLobbyPlayer).ShowInformation();
                    }
                }
                StartCoroutine(showResult());
            }
        }

        IEnumerator showResult()
        {
            if (NetworkServer.active)
                StartCoroutine(uploadMatchData());
            GameObject canvas = GameObject.Find("Canvas");
            Animator anim = canvas.GetComponent<Animator>();
            anim.SetTrigger("GameToResult");
            ResultInfo resultInfo = GameObject.Find("Result").GetComponent<ResultInfo>();
            int mainTotalScore = 0;
            int oppTotalScore = 0;
            for (int i = 0; i < s_Singleton.lobbySlots.Length; ++i)
            {
                MyLobbyPlayer cur = s_Singleton.lobbySlots[i] as MyLobbyPlayer;
                if (cur.isLocalPlayer)
                {
                    resultInfo.main[0].text = cur.round1Score.ToString();
                    resultInfo.main[1].text = cur.round2Score.ToString();
                    resultInfo.main[2].text = cur.round3Score.ToString();
                    mainTotalScore = cur.totalScore;
                }
                else
                {
                    resultInfo.opp[0].text = cur.round1Score.ToString();
                    resultInfo.opp[1].text = cur.round2Score.ToString();
                    resultInfo.opp[2].text = cur.round3Score.ToString();
                    oppTotalScore = cur.totalScore;
                }
            }
            int mainTotalCount = 0;
            int oppTotalCount = 0;
            //Time to wait Game to Result show total score
            yield return new WaitForSecondsRealtime(4.5f);
            while (mainTotalCount < mainTotalScore || oppTotalCount < oppTotalScore)
            {
                yield return null;
                if (mainTotalCount < mainTotalScore)
                    ++mainTotalCount;
                if (oppTotalCount < oppTotalScore)
                    ++oppTotalCount;
                resultInfo.main[3].text = mainTotalCount.ToString();
                resultInfo.opp[3].text = oppTotalCount.ToString();
            }
            yield return new WaitForSecondsRealtime(1.0f);
            if (mainTotalScore > oppTotalScore)
                resultInfo.resultAnim.SetTrigger("Win");
            else if (mainTotalScore == oppTotalScore)
            {
                resultInfo.resultAnim.SetTrigger("Draw");
            }
            else
                resultInfo.resultAnim.SetTrigger("Lose");
            //Time to animate above animation
            yield return new WaitForSecondsRealtime(4.0f);
            //Announce to server that this client have done result animation
            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                if (lobbySlots[i].isLocalPlayer)
                {
                    (lobbySlots[i] as MyLobbyPlayer).CmdDoneLoadResultScene();
                }
            }
            MainPlayer.instance.updatePlayerData();
            //Wait to out game at the same time
            bool wait = false;
            while (!wait)
            {
                yield return null;
                wait = (lobbySlots[0] as MyLobbyPlayer).loadResultScene && (lobbySlots[1] as MyLobbyPlayer).loadResultScene;
            }
            //should be call on both or client won't return to arena scene
            s_Singleton.backDelegate();
            //Client dont auto back to arena scene after the game
            if (!NetworkServer.active)
                SceneManager.LoadSceneAsync("Arena");
        }

        public override void OnLobbyServerSceneChanged(string sceneName)
        {
            base.OnLobbyServerSceneChanged(sceneName);
        }

        //Set up delegate for back button function
        public delegate void BackButtonDelegate();
        public BackButtonDelegate backDelegate;


        // ----------------- Server management

        public void StopHostClbk()
        {
            _disconnectServer = true;
            matchMaker.DestroyMatch((NetworkID)_currentMatchID, 0, OnDestroyMatch);
            isInGame = false;
            players.Clear();
        }

        public void StopClientClbk()
        {
            //matchMaker.DropConnection((NetworkID)_currentMatchID, _currentNodeID, 0, OnDropConnection);
            StopMatchMaker();
            StopClient();
            isInGame = false;
        }

        public override void OnStartHost()
        {
            base.OnStartHost();

            backDelegate = StopHostClbk;
        }

        //Add store current match ID
        public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            base.OnMatchCreate(success, extendedInfo, matchInfo);
            if (success)
            {
                _currentMatchID = (System.UInt64)matchInfo.networkId;
                isInGame = true;
            }
            else
                s_Singleton.backDelegate();
        }

        public override void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            base.OnMatchJoined(success, extendedInfo, matchInfo);
            if (success)
            {
                _currentMatchID = (System.UInt64)matchInfo.networkId;
                _currentNodeID = matchInfo.nodeId;
                isInGame = true;
                backDelegate = StopClientClbk;
            }
            else
                s_Singleton.backDelegate();
        }

        //handle disconnect server situation
        public override void OnDestroyMatch(bool success, string extendedInfo)
        {
            base.OnDestroyMatch(success, extendedInfo);
            if (_disconnectServer)
            {
                StopMatchMaker();
                StopHost();
            }
        }

        // ----------------- Server callbacks ------------------

        public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
        {
            GameObject obj = Instantiate(lobbyPlayerPrefab.gameObject) as GameObject;

            return obj;
        }


        public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
        {
            //This hook allows you to apply state data from the lobby-player to the game-player
            //just subclass "LobbyHook" and add it to the lobby object.

            return true;
        }
        // --- Countdown management
        //This is called on the server when all the players in the lobby are ready.
        public override void OnLobbyServerPlayersReady()
        {
            bool allready = true;
            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                allready &= lobbySlots[i].readyToBegin;
            }

            if (allready)
            {
                //Pick 3 games will be played
                Select3Games();
                StartCoroutine(ServerCountdownCoroutine());
            }
        }

        public IEnumerator ServerCountdownCoroutine()
        {
            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                (lobbySlots[i] as MyLobbyPlayer).RpcUpdateCountdown(1);
                (lobbySlots[i] as MyLobbyPlayer).RpcDeactiveInfoPanel();
            }
            //Time to animate match success
            yield return new WaitForSecondsRealtime(2.0f);

            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                (lobbySlots[i] as MyLobbyPlayer).RpcUpdateCountdown(0);
                (lobbySlots[i] as MyLobbyPlayer).RpcShowInformation();
            }
            //Time to animate lobby to game
            yield return new WaitForSecondsRealtime(5.0f);
            ServerChangeScene(gameToPlay[0]);
        }

        // ----------------- Client callbacks ------------------

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
        }

        // ----------------- Find match ------------------

        public void findMatch()
        {
            //Check if there are existed matches
            //If yes, join it else create new match
            infoPanel.display();
            StartMatchMaker();
            matchMaker.ListMatches(0, 1, "", true, 0, 0, OnMatchList);
        }

        public override void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
        {  
            if (matches.Count == 0)
            {
                //Script for run as a host
                matchMaker.CreateMatch(
                    //can use different number to manage existed lobby
                    "",
                    (uint)maxPlayers,
                    true,
                    "", "", "",
                    0, //eloScoreForMatch: can use to match player with the same rank
                    0,
                    OnMatchCreate);
                backDelegate = StopHost;
                infoPanel.addBackFunction();
            }
            else
            {
                matchMaker.JoinMatch(matches[0].networkId, "", "", "", 0, 0, OnMatchJoined);
                backDelegate = StopClient;
                infoPanel.addBackFunction();
            }
        }

        void Select3Games()
        {
            string[] vocabularyGames = { "BubblePop" };
            string[] listeningGames = { "WordBlock", "PearlHunter" };
            string[] grammarGames = { "GhostBuster" };
            int rand;
            rand = Random.Range(0, vocabularyGames.Length);
            gameToPlay[0] = vocabularyGames[rand];
            rand = Random.Range(0, listeningGames.Length);
            gameToPlay[1] = listeningGames[rand];
            rand = Random.Range(0, grammarGames.Length);
            gameToPlay[2] = grammarGames[rand];
        }

        public void OnServerNextGame()
        {
            if (NetworkServer.active)
            {
                foreach (Player p in players)
                {
                    p.RpcStartLoadingScene(currentRound);
                }
                ServerChangeScene(gameToPlay[currentRound]); 
            }
            ++currentRound;
        }

        public void OnServerEndGame()
        {
            if (NetworkServer.active)
            {
                ServerChangeScene(lobbyScene);
            }
        }

        public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection conn, short playerControllerId)
        {
            GameObject gamePlayer;
            Transform startPos = GetStartPosition();
            if (startPos != null)
            {
                gamePlayer = Instantiate(gamePlayerPrefab, startPos.position, startPos.rotation);
            }
            else
            {
                gamePlayer = Instantiate(gamePlayerPrefab, Vector3.zero, Quaternion.identity);
            }
            players.Add(gamePlayer.GetComponent<Player>());
            return gamePlayer;
        }

        //Increase rating if still in-game

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            if (isInGame)
            {
                disconnectPanel.display();
                StartCoroutine(uploadMatchData());
            }
            base.OnClientDisconnect(conn);
            StopClientClbk();
        }

        //Increase rating if still in-game

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            if (isInGame)
            {
                disconnectPanel.display();
                StartCoroutine(uploadMatchData());
            }
            base.OnServerDisconnect(conn);
            StopHostClbk();
        }

        //Quit match in right way

        private void OnApplicationQuit()
        {
            if (isInGame)
            {
                s_Singleton.backDelegate();
            }
        }

        //update player's score round 3 and total

        public void OnServerUpdateLastRoundData()
        {
            if (NetworkServer.active)
            {
                foreach (Player p in players)
                {
                    p.RpcStartLoadingScene(currentRound);
                }
            }
        }

        //Transfer data from player to lobby player before players are deleted when back to lobby scene
        public void OnServerUpdateLobbyData()
        {
            if (NetworkServer.active)
            {
                for (int i = 0; i < lobbySlots.Length; ++i)
                {
                    (lobbySlots[i] as MyLobbyPlayer).UpdateDataFromServer();
                }
            }
        }

        //Unlist this match from find list
        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            base.OnServerAddPlayer(conn, playerControllerId);
            if (NetworkServer.connections.Count == 2)
            {
                matchMaker.SetMatchAttributes((NetworkID)_currentMatchID, false, 0, OnSetMatchAttributes);
            }
        }

        IEnumerator uploadMatchData()
        {
            //If still in game, get data from player else get data from lobby player
            if (isInGame)
            {
                Player[] players = FindObjectsOfType<Player>();
                string disconnectPlayer;
                if (players[0].isLocalPlayer)
                {
                    //Condition to plus rating when opp disconnected
                    if ((players[0].score + players[0].totalScore) > (players[1].score + players[1].totalScore + 10))
                        MainPlayer.instance.playerData.rating += 5;
                    else
                        yield break;
                    disconnectPlayer = "player2";
                }
                else
                {
                    if ((players[1].score + players[1].totalScore) > (players[0].score + players[0].totalScore + 10))
                        MainPlayer.instance.playerData.rating += 5;
                    else
                        yield break;
                    disconnectPlayer = "player1";
                }
                string dataPlayer1 = JsonUtility.ToJson(players[0]);
                string dataPlayer2 = JsonUtility.ToJson(players[1]);
                using (UnityWebRequest www = UnityWebRequest.Put("http://powerlishproject.dx.am/uploadMatchData.php", dataPlayer1 + "/" + dataPlayer2 + "/" + disconnectPlayer + "/" + localNick + "/" + oppNick))
                {
                    www.SetRequestHeader("Content-Type", "application/json");
                    yield return www.SendWebRequest();

                    if (www.isNetworkError || www.isHttpError)
                    {
                        Debug.Log(www.error);
                    }
                    else
                    {

                    }
                }
            }
            else
            {
                string dataPlayer1 = JsonUtility.ToJson(lobbySlots[0]);
                string dataPlayer2 = JsonUtility.ToJson(lobbySlots[1]);
                using (UnityWebRequest www = UnityWebRequest.Put("http://powerlishproject.dx.am/uploadMatchData.php", dataPlayer1 + "/" + dataPlayer2 + "/None/" + localNick + "/" + oppNick))
                {
                    www.SetRequestHeader("Content-Type", "application/json");
                    yield return www.SendWebRequest();

                    if (www.isNetworkError || www.isHttpError)
                    {
                        Debug.Log(www.error);
                    }
                    else
                    {

                    }
                }
            }
        }
    }
}