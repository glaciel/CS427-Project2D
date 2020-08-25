using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    Rigidbody2D rb2D;
    public string answer;
    BubblepopGameController gameController;
    float ForceValue = 15.0f;
    Animator anim;
    public Question question;
    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        rb2D.AddForce(new Vector2(Random.Range(-1.0f, 1.0f) * ForceValue, 0));
        gameController = FindObjectOfType<BubblepopGameController>();
        //Get word for this bubble
        //First random unit with latest unit have highest probability
        int randUnit;
        if (gameController.practiceMode)
        {
            randUnit = gameController.practiceUnit;
        }
        else
        {
            randUnit = gameController.pickRandUnit(gameController.vocabList.Length);
        }
        //Then choose random question in that unit
        int randQuestion = Random.Range(0, gameController.questionsData[randUnit].Count);
        question = gameController.questionsData[randUnit][randQuestion];
        answer = question.content[0];
        gameController.questionsData[randUnit].RemoveAt(randQuestion);
        //Get this answer's pronunciation
        AudioClip audioClip = Resources.Load<AudioClip>("Vocabulary/" + answer);
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = audioClip;
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Wall")
        {
            rb2D.gravityScale *= -1;
            rb2D.AddForce(new Vector2(0, Random.Range(-0.1f, 0.1f) * ForceValue));
        }

        if (col.gameObject.tag == "SideWall")
        {
            rb2D.AddForce(new Vector2(Random.Range(-0.7f, 0.7f) * ForceValue, 0));
        }
        if (col.gameObject.tag == "Bubble")
        {
            rb2D.AddForce(new Vector2(Random.Range(-0.8f, 0.8f) * ForceValue, Random.Range(-0.5f, 0.5f) * ForceValue));
        }
        isTouched();
    }

    public void isTouched()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Wobble"))
        {
            anim.SetTrigger("Touched");
        }
    }

    public void touchedByPlayer()
    {
        gameController.touchedBubble = this;
    }

    //Random function with different probabilities
    int Choose(float[] probs)
    {

        float total = 0;

        foreach (float elem in probs)
        {
            total += elem;
        }

        float randomPoint = Random.value * total;

        for (int i = 0; i < probs.Length; i++)
        {
            if (randomPoint < probs[i])
            {
                return i;
            }
            else
            {
                randomPoint -= probs[i];
            }
        }
        return probs.Length - 1;
    }
}
