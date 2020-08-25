using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerController : MonoBehaviour
{
    public string previousAnswer;
    public GameObject alphabetPrefab;
    public Sprite[] alphabetSprites;
    public Sprite[] numberSprites;
    public WordBlockGameController wbGameController;
    public List<GameObject> alphabets;
    // Start is called before the first frame update
    void Start()
    {
        alphabets = new List<GameObject>();
        previousAnswer = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateDisplayedWord(string currentAnswer)
    {
        //If player add more word to game
        if (currentAnswer.Length > previousAnswer.Length)
        {
            alphabets.Add(Instantiate(alphabetPrefab, wbGameController.volcano.transform));
            SpriteRenderer sr = alphabets[alphabets.Count - 1].GetComponent<SpriteRenderer>();
            char cur = char.ToLower(currentAnswer[currentAnswer.Length - 1]);
            if (cur == ' ')
                sr.sprite = alphabetSprites[26];
            else if (cur < 'a')
                sr.sprite = numberSprites[int.Parse(cur.ToString())];
            else
                sr.sprite = alphabetSprites[cur - 97];
        }
        else if (currentAnswer.Length < previousAnswer.Length)
        {
            Animator anim = alphabets[alphabets.Count - 1].GetComponent<Animator>();
            anim.SetTrigger("Fade");
            alphabets.RemoveAt(alphabets.Count - 1);
        }
        previousAnswer = currentAnswer;
    }
}
