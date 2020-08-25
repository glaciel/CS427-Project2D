using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BubblepopGameController : MiniGameController {
    [Serializable]
    public class BubblepopQuestion : Question
    {
        public override void getQuestionData(string line, int unit)
        {
            string[] data = System.Text.RegularExpressions.Regex.Split(line, ",");
            ID = int.Parse(data[0]);
            content = new string[1];
            content[0] = data[1];
            this.unit = unit;
        }
    }

    public GameObject bubblePrefab;
    public GameObject bubbles;
    public List<BubblepopQuestion>[] questionsData;
    public TextAsset[] vocabList;
    List<Player> players;
    public InputField playerInput;
    public GameObject popEffect;
    public Text correctAnswer;
    public Bubble touchedBubble;
    public AudioSource correctSound;
    public AudioSource wrongSound;
	// Use this for initialization
	void Start () {
        players = new List<Player>();
        questionsData = new List<BubblepopQuestion>[vocabList.Length];
        for (int i = 0; i < questionsData.Length; ++i)
        {
            questionsData[i] = new List<BubblepopQuestion>();
            getQuestionList(i);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (!startGame)
            return;
        if (players.Count < 2 && Player.FindObjectsOfType<Player>().Length == 2)
        {
            players.AddRange(Player.FindObjectsOfType<Player>());
        }
        //If there is a pop bubble, spawn new one
        if (bubbles.transform.childCount < 5)
        {
            spawnBubble();
        }
    }

    void spawnBubble()
    {
        Instantiate(bubblePrefab, new Vector3(UnityEngine.Random.Range(-3.0f, 3.0f), UnityEngine.Random.Range(-4.0f, 3.0f)), Quaternion.identity, bubbles.transform);
    }

    public void checkAnswer(string input)
    {
        //Check answer from all bubbles
        Bubble[] allBubble = FindObjectsOfType<Bubble>();
        for (int i = 0; i < allBubble.Length; ++i)
        {
            if (input == allBubble[i].answer)
            {
                //Update research data based on right or wrong answer + user dont type randomly
                if (touchedBubble != null)
                {
                    correctSound.Play();
                    ++totalQuestion;
                    updatePlayerPoint(true);
                    updatePlayerQuestionData(true);
                    Animator anim = allBubble[i].GetComponent<Animator>();
                    Instantiate(popEffect, allBubble[i].transform.position, Quaternion.identity, bubbles.transform);
                    anim.SetTrigger("CorrectAnswer");
                    playerInput.text = "";
                    if (correctAnswer != null)
                        correctAnswer.text = input;
                    foreach (Player player in players)
                    {
                        if (player.isLocalPlayer)
                        {
                            player.CmdIncreaseScore();
                            break;
                        }
                    }
                    return;
                }
            }
        }
        Handheld.Vibrate();
        if (touchedBubble != null)
        {
            wrongSound.Play();
            updatePlayerPoint(false);
            updatePlayerQuestionData(false);
        }
    }

    public override void updatePlayerPoint(bool correct)
    {
        ++MainPlayer.instance.playerData.vocabularyPoint.meet;
        if (correct)
            ++MainPlayer.instance.playerData.vocabularyPoint.correct;
        MainPlayer.instance.playerData.vocabularyPoint.updateValue();
    }

    public override void updatePlayerQuestionData(bool correct)
    {
        base.updatePlayerQuestionData(correct);
        MainPlayer.instance.playerData.updateMetQuestionsData(MainPlayer.instance.playerData.metVocabulary, touchedBubble.question, correct);
    }

    //load vocab list from text file
    void getQuestionList(int index)
    {
        List<string> data = new List<string>();
        data.AddRange(System.Text.RegularExpressions.Regex.Split(vocabList[index].text, "\n|\r\n"));
        int unit = int.Parse(data[0]);
        data.RemoveAt(0);
        foreach (string line in data)
        {
            BubblepopQuestion q = new BubblepopQuestion();
            q.getQuestionData(line, unit);
            questionsData[index].Add(q);
        }
    }
}