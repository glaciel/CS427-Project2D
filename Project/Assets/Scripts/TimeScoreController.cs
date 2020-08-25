using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TimeScoreController : MonoBehaviour
{
    public Text timeText;
    public GameObject scoreLeft;
    public GameObject scoreRight;
    public Text announce;
    public float playingTime;
    public MiniGameController gameController;
    public GameObject howToPlayPanel;
    GameObject canvas;
    float startTime;
    float leftRatio = 0.5f;
    float rightRatio = 0.5f;
    List<Player> players;
    GameObject mainCamera;
    void Start()
    {
        mainCamera = GameObject.Find("Main Camera");
        canvas = GameObject.Find("Canvas");
        players = new List<Player>();
        Text text = howToPlayPanel.GetComponentInChildren<Text>();
        text.text = "Round " + Prototype.NetworkLobby.MyLobbyManager.s_Singleton.currentRound.ToString();
        StartCoroutine(waitOtherPlayer());
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameController.startGame)
            return;
        if ((Time.realtimeSinceStartup - startTime) > playingTime)
        {
            if (!announce.gameObject.activeInHierarchy)
                endGame();
            return;
        }
        //recalculate left, right ratio
        float diff = 0.5f + (players[0].score - players[1].score) / 200.0f;
        if (leftRatio < diff)
            leftRatio += Time.deltaTime * 0.005f;
        else if (leftRatio > diff)
            leftRatio -= Time.deltaTime * 0.005f;
        if (rightRatio < (1 - diff))
            rightRatio += Time.deltaTime * 0.005f;
        else if (rightRatio > (1 - diff))
            rightRatio -= Time.deltaTime * 0.005f;
        UpdateScoreDisplay(scoreLeft, leftRatio);
        UpdateScoreDisplay(scoreRight, rightRatio);
        timeText.text = Mathf.FloorToInt(Time.realtimeSinceStartup - startTime).ToString();
    }

    void UpdateScoreDisplay(GameObject side, float ratio)
    {
        RectTransform rect = canvas.GetComponent<RectTransform>();
        float min, max;
        Rect scaleRect = RectTransformToScreenSpace(rect);
        if (side.name == "ScoreLeft")
        {
            min = scaleRect.xMin;
            max = scaleRect.xMax;
        }
        else
        {
            min = scaleRect.xMax;
            max = scaleRect.xMin;
        }
        Text text;
        text = side.GetComponentInChildren<Text>();
        if (side.name == "ScoreLeft")
            text.text = players[0].score.ToString();
        else
            text.text = players[1].score.ToString();
        side.transform.position = new Vector3(Mathf.Lerp(min, max, ratio), side.transform.position.y, 0);
    }

    void endGame()
    {
        Resources.UnloadUnusedAssets();
        InputField playerInput = FindObjectOfType<InputField>();
        if (playerInput != null)
        {
            if (playerInput.touchScreenKeyboard != null)
            {
                if (playerInput.touchScreenKeyboard.active)
                    playerInput.touchScreenKeyboard.active = false;
            }
        }
        timeText.text = playingTime.ToString();
        GraphicRaycaster graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
        graphicRaycaster.enabled = false;
        Physics2DRaycaster physics2DRaycaster = mainCamera.GetComponent<Physics2DRaycaster>();
        if (physics2DRaycaster != null)
            physics2DRaycaster.enabled = false;
        //need to plus current score because total score just in round 2
        Player higherScore;
        int leftScore = players[0].totalScore + players[0].score;
        int rightScore = players[1].totalScore + players[1].score;
        if (leftScore > rightScore)
            higherScore = players[0];
        else if (leftScore == rightScore)
            higherScore = null;
        else
            higherScore = players[1];
        if (Prototype.NetworkLobby.MyLobbyManager.s_Singleton.currentRound == 3)
        {
            Prototype.NetworkLobby.MyLobbyManager.s_Singleton.OnServerUpdateLastRoundData();
            //Change score from players to lobby players
            Invoke("UpdateLobbyData", 2.0f);
            if (higherScore != null)
            {
                if (higherScore.isLocalPlayer)
                    StartCoroutine(BackToLobbyScene(true));
                else
                    StartCoroutine(BackToLobbyScene(false));
            }
            else
                StartCoroutine(BackToLobbyScene(false));
        }
        else
        {
            Invoke("nextGame", 1.5f);
        }
        announce.text = "Time's up";
        announce.gameObject.SetActive(true);
    }

    public static Rect RectTransformToScreenSpace(RectTransform transform)
    {
        Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
        return new Rect((Vector2)transform.position - (size * 0.5f), size);
    }

    IEnumerator BackToLobbyScene(bool result)
    {
        yield return new WaitForSecondsRealtime(3.5f);
        if (result)
            MainPlayer.instance.playerData.rating += 5;
        Prototype.NetworkLobby.MyLobbyManager.s_Singleton.OnServerEndGame();
    }

    void nextGame()
    {
        Prototype.NetworkLobby.MyLobbyManager.s_Singleton.OnServerNextGame();
    }

    void UpdateLobbyData()
    {
        Prototype.NetworkLobby.MyLobbyManager.s_Singleton.OnServerUpdateLobbyData();
    }

    IEnumerator countdownStartTime()
    {
        yield return new WaitForSecondsRealtime(4.0f);
        //Change loaded scene to false to avoid next game change to fast and detect it's true before it change to false
        players[0].CmdStartLoadingScene();
        howToPlayPanel.SetActive(false);
        startTime = Time.realtimeSinceStartup;
        gameController.startGame = true;
    }

    IEnumerator waitOtherPlayer()
    {
        while(FindObjectsOfType<Player>().Length != 2)
        {
            yield return null;
        }
        players.AddRange(FindObjectsOfType<Player>());
        if (!players[0].isLocalPlayer)
            players.Reverse();
        while (!players[0].loadedScene || !players[1].loadedScene)
            yield return null;
        StartCoroutine(countdownStartTime());
    }
}
