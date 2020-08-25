using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PearlHunterGameController : MiniGameController
{
    [System.Serializable]
    public class PearlHunterQuestion : Question
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
    public GameObject trueOysterPrefab;
    public GameObject falseOysterPrefab;
    public GameObject rockDark;
    public GameObject rockMedium;
    public GameObject rockLight;
    public GameObject rockTopDark;
    public Scrollbar scriptScrollbar;
    public GameObject trueOyster;
    public GameObject falseOyster;
    GameObject canvas;
    bool spawnedOyster = false;
    bool fasten = false;
    float offset = 0.8f;
    float speed = 0.5f;
    List<Player> players;
    public Text correctAnswer;
    public List<PearlHunterQuestion> questionsData;
    public GameObject[] fishPrefabs;
    public GameObject fish;
    public AudioSource correctSound;
    public AudioSource wrongSound;
    public int curQuestion = 0;
    //Choose 1 unit for this game to make content relevant
    int playUnit;
    // Start is called before the first frame update
    void Start()
    {
        questionsData = new List<PearlHunterQuestion>();
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
                speed += Time.deltaTime * 1.6f;
                setSpeedAll();
            }
            else
            {
                fasten = false;
                if ((curQuestion * 2 + 1) > questionsData[currentPlayingNum].content.Length)
                {
                    questionsData.RemoveAt(currentPlayingNum);
                    nextAudio = true;
                    spawnedOyster = false;
                    curQuestion = 0;
                    displayingScript.text = "";
                }
                else
                    displayQuestion();
            }
        }
        if (!fasten && speed > 0.5f)
        {
            speed -= Time.deltaTime * 0.4f;
            setSpeed(rockDark, speed);
            setSpeed(rockMedium, speed);
            setSpeed(rockLight, speed);
            setSpeed(rockTopDark, speed);
            setSpeed(fish, speed);
        }
        if (nextAudio)
        {
            playRandomClip();
            nextAudio = false;
        }

        if (playingSource.clip != null && (playingSource.time / playingSource.clip.length) > offset)
        {
            if (!spawnedOyster)
            {
                spawnedOyster = true;
                displayQuestion();
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
            PearlHunterQuestion q = new PearlHunterQuestion();
            q.getQuestionData(line, unit);
            questionsData.Add(q);
        }
        if (practiceMode)
            startGame = true;
    }

    public void checkAnswer(bool correct)
    {
        if (correct)
        {
            correctSound.Play();
            updatePlayerPoint(true);
            updatePlayerQuestionData(true);
            resumeAll();
            fasten = true;
            //Show correct answer in practice mode
            if (correctAnswer != null)
            {
                if (questionsData[currentPlayingNum].content[curQuestion * 2 + 1] == "T")
                {
                    correctAnswer.text = "True";
                }
                else
                {
                    correctAnswer.text = "False";
                }  
            }
            //If in arena mode then update player score
            foreach (Player player in players)
            {
                if (player.isLocalPlayer)
                {
                    player.CmdIncreaseScore();
                    break;
                }
            }
        }
        else
        {
            wrongSound.Play();
            updatePlayerPoint(false);
            updatePlayerQuestionData(false);
            Handheld.Vibrate();
            resumeAll();
            fasten = true;
        }
        ++curQuestion;
        ++totalQuestion;
    }

    public void stopAll()
    {
        pauseAnim(rockDark);
        pauseAnim(rockMedium);
        pauseAnim(rockLight);
        pauseAnim(rockTopDark);
    }

    void resumeAll()
    {
        resumeAnim(rockDark);
        resumeAnim(rockMedium);
        resumeAnim(rockLight);
        resumeAnim(rockTopDark);
    }

    void setSpeedAll()
    {
        setSpeed(rockDark, speed);
        setSpeed(rockMedium, speed);
        setSpeed(rockLight, speed);
        setSpeed(rockTopDark, speed);
        setSpeed(trueOyster, speed);
        setSpeed(falseOyster, speed);
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
        playingSource.clip = Resources.Load<AudioClip>("Listening/Unit" + (playUnit + 1).ToString() + "/" + questionsData[currentPlayingNum].ID.ToString());
        playingSource.Play();
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

    public void setPlayUnit(int unit)
    {
        playUnit = unit;
    }

    void displayQuestion()
    {
        displayingScript.text = questionsData[currentPlayingNum].content[curQuestion * 2];
        modifyTextWidth();
        trueOyster = Instantiate(trueOysterPrefab, canvas.transform);
        falseOyster = Instantiate(falseOysterPrefab, canvas.transform);
        if (questionsData[currentPlayingNum].content[curQuestion * 2 + 1] == "T")
        {
            Animator anim = trueOyster.GetComponent<Animator>();
            anim.SetBool("Correct", true);
        }
        else
        {
            Animator anim = falseOyster.GetComponent<Animator>();
            anim.SetBool("Correct", true);
        }
    }
}
