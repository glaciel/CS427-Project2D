using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordBlockGameController: MiniGameController
{
    [System.Serializable]
    public class WordBlockQuestion : Question
    {
        public override void getQuestionData(string line, int unit)
        {
            base.getQuestionData(line, unit);
            string[] data = System.Text.RegularExpressions.Regex.Split(line, ";");
            ID = int.Parse(data[0]);
            int length = data.Length - 1;
            content = new string[length];
            for (int i = 0; i < length; ++i)
            {
                content[i] = data[i + 1];
            }
            this.unit = unit;
        }
    }
    public int currentPlayingNum;
    public AudioSource playingSource;
    public TextAsset[] questionList;
    public Text displayingScript;
    public bool nextAudio = true;
    public GameObject volcanoPrefab;
    public GameObject rockDark;
    public GameObject rockMedium;
    public RockLightController rockLight;
    public Scrollbar scriptScrollbar;
    AnswerController answerController;
    public GameObject volcano;
    GameObject canvas;
    InputField PlayerInput;
    bool spawnedVolcano = false;
    bool fasten = false;
    float offset;
    float speed = 0.5f;
    List<Player> players;
    public Text correctAnswer;
    public List<WordBlockQuestion> questionsData;
    public GameObject[] fishPrefabs;
    GameObject fish;
    public AudioSource correctSound;
    public AudioSource wrongSound;
    //Choose 1 unit for this game to make content relevant
    int playUnit;
    // Start is called before the first frame update
    void Start()
    {
        questionsData = new List<WordBlockQuestion>();
        //First random unit with latest unit have highest probability
        if (!practiceMode)
        {
            playUnit = pickRandUnit(questionList.Length);
            getQuestionList(playUnit);
        }
        canvas = GameObject.Find("Canvas");
        foreach (GameObject go in fishPrefabs)
        {
            if (go.name == MainPlayer.instance.fishName)
            {
                fish = Instantiate(go, canvas.transform);
                break;
            }
        }
        players = new List<Player>();
        answerController = GameObject.Find("AnswerController").GetComponent<AnswerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!startGame)
            return;
        if (players.Count < 2 && FindObjectsOfType<Player>().Length == 2)
        {
            players.AddRange(FindObjectsOfType<Player>());
        }
        if (fasten)
        {
            if (speed < 1.0f)
            {
                speed += Time.deltaTime * 1.5f;
                setSpeedAll();
            }
            else
                fasten = false;
        }
        if (!fasten && speed > 0.5f)
        {
            speed -= Time.deltaTime * 0.4f;
            setSpeed(rockDark, speed);
            setSpeed(rockMedium, speed);
            setSpeed(rockLight.current, speed);
            setSpeed(fish, speed);
        }
        if (nextAudio)
        {
            playRandomClip();
            nextAudio = false;
        }

        if (playingSource.clip != null && (playingSource.time / playingSource.clip.length) > offset)
        {
            if (!spawnedVolcano)
            {
                spawnedVolcano = true;
                volcano = Instantiate(volcanoPrefab, new Vector3(10, 0), Quaternion.identity, canvas.transform);
                setUpVolcano();
            }
                speed -= Time.deltaTime * 0.1f;
                setSpeedAll();
        }
    }

    public void getQuestionList(int index)
    {
        List<string> data = new List<string>();
        data.AddRange(System.Text.RegularExpressions.Regex.Split(questionList[index].text, "\n|\r\n"));
        int unit = int.Parse(data[0]);
        data.RemoveAt(0);
        foreach (string line in data)
        {
            WordBlockQuestion q = new WordBlockQuestion();
            q.getQuestionData(line, unit);
            questionsData.Add(q);
        }
        if (practiceMode)
            startGame = true;
    }

    public void checkAnswer(string input)
    {
        if (input == questionsData[currentPlayingNum].content[1])
        {
            correctSound.Play();
            updatePlayerPoint(true);
            updatePlayerQuestionData(true);
            nextAudio = true;
            resumeAll();
            fasten = true;
            spawnedVolcano = false;
            if (correctAnswer != null)
                correctAnswer.text = input;
            //If in arena mode then update player score
            foreach (Player player in players)
            {
                if (player.isLocalPlayer)
                {
                    player.CmdIncreaseScore();
                    break;
                }
            }
            questionsData.RemoveAt(currentPlayingNum);
        }
        else
        {
            wrongSound.Play();
            updatePlayerPoint(false);
            updatePlayerQuestionData(false);
            foreach (GameObject alphabet in answerController.alphabets)
            {
                Animator anim = alphabet.GetComponent<Animator>();
                anim.SetTrigger("Fade");
            }
            VolcanoSteam vs = volcano.GetComponentInChildren<VolcanoSteam>();
            vs.eruption();
            Handheld.Vibrate();
        }
        answerController.alphabets.Clear();
        answerController.previousAnswer = "";
        PlayerInput.text = "";
    }

    public void stopAll()
    {
        pauseAnim(rockDark);
        pauseAnim(rockMedium);
        pauseAnim(rockLight.current);
        pauseAnim(volcano);
        Transform clickHere = volcano.transform.Find("ClickHere");
        clickHere.gameObject.SetActive(true);
    }

    void resumeAll()
    {
        resumeAnim(rockDark);
        resumeAnim(rockMedium);
        resumeAnim(rockLight.current);
        resumeAnim(volcano);
    }

    void setSpeedAll()
    {
        setSpeed(rockDark, speed);
        setSpeed(rockMedium, speed);
        setSpeed(rockLight.current, speed);
        setSpeed(volcano, speed);
        setSpeed(fish, speed);
    }

    void pauseAnim(GameObject gameObject)
    {
        Animator anim = gameObject.GetComponent<Animator>();
        anim.enabled = false;
    }

    void resumeAnim(GameObject gameObject)
    {
        Animator anim = gameObject.GetComponent<Animator>();
        anim.enabled = true;
    }

    void setSpeed(GameObject gameObject, float value)
    {
        Animator anim = gameObject.GetComponent<Animator>();
        anim.SetFloat("Speed", value);
    }

    void playRandomClip()
    {
        //random audio index will be play at the beginning
        currentPlayingNum = Random.Range(0, questionsData.Count);
        ++totalQuestion;
        playingSource.clip = Resources.Load<AudioClip>("Listening/Unit" + (playUnit + 1).ToString() + "/" + questionsData[currentPlayingNum].ID.ToString());
        playingSource.Play();
        displayingScript.text = questionsData[currentPlayingNum].content[0];
        modifyTextWidth();
        setAudioOffset();
    }

    void setAudioOffset()
    {
        if (playingSource.clip.length < 8.0f)
        {
            offset = 0.15f;
        }
        else if (playingSource.clip.length < 3.5f)
            offset = 0.0f;
        else
        {
            offset = 0.45f;
        }
    }

    void modifyTextWidth()
    {
        List<string> countWord = new List<string>();
        countWord.AddRange(System.Text.RegularExpressions.Regex.Split(displayingScript.text, " "));
        displayingScript.rectTransform.sizeDelta = new Vector2(128 * countWord.Count, 133);
        scriptScrollbar.value = 0;
    }

    public override void updatePlayerPoint(bool correct)
    {
        ++MainPlayer.instance.playerData.listeningPoint.meet;
        if (correct)    
            ++MainPlayer.instance.playerData.listeningPoint.correct;
        MainPlayer.instance.playerData.listeningPoint.updateValue();
    }

    public override void updatePlayerQuestionData(bool correct)
    {
        base.updatePlayerQuestionData(correct);
        MainPlayer.instance.playerData.updateMetQuestionsData(MainPlayer.instance.playerData.metListening, questionsData[currentPlayingNum], correct);
    }

    public void passQuestion()
    {
        nextAudio = true;
        resumeAll();
        fasten = true;
        spawnedVolcano = false;
        updatePlayerPoint(false);
        updatePlayerQuestionData(false);
        answerController.alphabets.Clear();
        answerController.previousAnswer = "";
        PlayerInput.text = "";
        questionsData.RemoveAt(currentPlayingNum);
    }
    
    public void replayAudio()
    {
        if (!playingSource.isPlaying)
            playingSource.Play();
    }

    void setUpVolcano()
    {
        PlayerInput = volcano.GetComponentInChildren<InputField>();
        PlayerInput.onEndEdit.AddListener(checkAnswer);
        PlayerInput.onValueChanged.AddListener(answerController.updateDisplayedWord);
        Button replayButton = volcano.gameObject.transform.Find("ReplayButton").gameObject.GetComponent<Button>();
        replayButton.onClick.AddListener(replayAudio);
        Button passButton = volcano.gameObject.transform.Find("PassButton").gameObject.GetComponent<Button>();
        passButton.onClick.AddListener(passQuestion);
    }

    public void setPlayUnit(int unit)
    {
        playUnit = unit;
    }
}