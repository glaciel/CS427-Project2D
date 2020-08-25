using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GhostBusterGameController : MiniGameController
{
    [System.Serializable]
    public class GhostBusterQuestion : Question
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
    public TextAsset[] questionList;
    public List<GhostBusterQuestion>[] questionsData;
    public Text Question;
    public GhostBusterQuestion currentQuestion;
    public string currentAnswer;
    public GameObject jellyfishPrefab;
    public bool nextQuestion = true;
    public Scrollbar scriptScrollbar;
    GameObject canvas;
    public GameObject[] fishPrefabs;
    public GameObject fish;
    // Start is called before the first frame update
    void Start()
    {
        questionsData = new List<GhostBusterQuestion>[questionList.Length];
        for (int i = 0; i < questionsData.Length; ++i)
        {
            questionsData[i] = new List<GhostBusterQuestion>();
            getQuestionList(i);
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
    }

    // Update is called once per frame
    void Update()
    {
        if (!startGame)
            return;
        if (nextQuestion)
        {
            getQuestion();
            spawnJellyfishes();
        }
    }

    void getQuestionList(int index)
    {
        List<string> data = new List<string>();
        data.AddRange(System.Text.RegularExpressions.Regex.Split(questionList[index].text, "\n|\r\n"));
        int unit = int.Parse(data[0]);
        data.RemoveAt(0);
        foreach (string line in data)
        {
            GhostBusterQuestion q = new GhostBusterQuestion();
            q.getQuestionData(line, unit);
            questionsData[index].Add(q);
        }
    }

    void getQuestion()
    {
        int randUnit;
        nextQuestion = false;
        if (practiceMode)
        {
            randUnit = practiceUnit;
        }
        else
        {
            randUnit = pickRandUnit(questionList.Length);
        }
        int rand = Random.Range(0, questionsData[randUnit].Count);
        currentQuestion = questionsData[randUnit][rand];
        Question.text = currentQuestion.content[0];
        questionsData[randUnit].RemoveAt(rand);
        currentAnswer = currentQuestion.content[currentQuestion.content.Length - 1];
        modifyTextWidth();
        ++totalQuestion;
    }

    void spawnJellyfishes()
    {
        int choicesNumber = currentQuestion.content.Length - 2;
        for (int currentJellyfish = 1; currentJellyfish <= choicesNumber; ++currentJellyfish)
        {
            GameObject newJellyfish = Instantiate(jellyfishPrefab, new Vector3(11f, -4.5f + 6f * (choicesNumber + 1 - currentJellyfish) / choicesNumber, 0), Quaternion.identity, canvas.transform);
            if (choicesNumber == 4)
            {
                RectTransform rect = newJellyfish.GetComponent<RectTransform>();
                rect.localScale = new Vector3(0.75f, 0.75f, 1.0f);
            }
            Jellyfish j = newJellyfish.GetComponent<Jellyfish>();
            j.choice = currentQuestion.content[currentJellyfish];
            if (currentJellyfish == 1)
                j.triggerFishAnimIndex = "0";
            else
            {
                j.triggerFishAnimIndex = choicesNumber.ToString() + "_" + (currentJellyfish - 1).ToString();
            }
        }
    }

    void modifyTextWidth()
    {
        List<string> countWord = new List<string>();
        countWord.AddRange(System.Text.RegularExpressions.Regex.Split(Question.text, " "));
        Question.rectTransform.sizeDelta = new Vector2(130 * countWord.Count, 131);
        scriptScrollbar.value = 0;
    }

    public override void updatePlayerPoint(bool correct)
    {
        ++MainPlayer.instance.playerData.grammarPoint.meet;
        if (correct)
            ++MainPlayer.instance.playerData.grammarPoint.correct;
        MainPlayer.instance.playerData.grammarPoint.updateValue();
    }

    public override void updatePlayerQuestionData(bool correct)
    {
        base.updatePlayerQuestionData(correct);
        MainPlayer.instance.playerData.updateMetQuestionsData(MainPlayer.instance.playerData.metGrammar, currentQuestion, correct);
    }
}
