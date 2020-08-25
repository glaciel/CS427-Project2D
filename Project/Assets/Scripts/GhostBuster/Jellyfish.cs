using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Jellyfish : MonoBehaviour
{
    public string choice;
    GhostBusterGameController gameController;
    public Animator anim;
    GameObject mainCamera;
    AnimationClip animationClip;
    public string triggerFishAnimIndex;
    List<Player> players;
    public GameObject disappearEffect;
    Text correctAnswer;
    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("CorrectAnswer") != null)
            correctAnswer = GameObject.Find("CorrectAnswer").GetComponentInChildren<Text>();
        players = new List<Player>();
        gameController = FindObjectOfType<GhostBusterGameController>();
        EventTrigger trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { OnPointerClickDelegate((PointerEventData)data); });
        trigger.triggers.Add(entry);
        Text text = GetComponentInChildren<Text>();
        text.text = choice;
        anim = GetComponent<Animator>();
        if (choice == gameController.currentAnswer)
            anim.SetBool("Illusion", false);
        else
            anim.SetBool("Illusion", true);
        mainCamera = GameObject.Find("Main Camera");
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left * Time.deltaTime * 1.2f);
        if (players.Count < 2 && Player.FindObjectsOfType<Player>().Length == 2)
        {
            players.AddRange(Player.FindObjectsOfType<Player>());
        }
    }

    public void OnPointerClickDelegate(PointerEventData data)
    {
        checkAnswer();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Fish"))
        {
            //Run this function only on 1 jellyfish
            if (choice == gameController.currentAnswer)
            {
                Physics2DRaycaster physics2DRaycaster = mainCamera.GetComponent<Physics2DRaycaster>();
                if (physics2DRaycaster != null)
                    physics2DRaycaster.enabled = false;
                Jellyfish[] jellyfishes = FindObjectsOfType<Jellyfish>();
                if (jellyfishes.Length <= 4)
                {
                    gameController.updatePlayerPoint(false);
                    gameController.updatePlayerQuestionData(false);
                    gameController.nextQuestion = true;
                    for (int i = 0; i < jellyfishes.Length; ++i)
                    {
                        jellyfishes[i].anim.SetTrigger("Wrong");
                    }
                }
            }
        }

        if (collision.gameObject.CompareTag("Beam"))
        {
            anim.SetTrigger("Hitted");
        }
    }

    public void checkAnswer()
    {
        Physics2DRaycaster physics2DRaycaster = mainCamera.GetComponent<Physics2DRaycaster>();
        if (physics2DRaycaster != null)
            physics2DRaycaster.enabled = false;
        Animator fishAnim = gameController.fish.GetComponent<Animator>();
        fishAnim.SetTrigger(triggerFishAnimIndex);
    }

    void illusionHitted()
    {
        gameController.updatePlayerPoint(false);
        gameController.updatePlayerQuestionData(false);
        Jellyfish[] jellyfishes = FindObjectsOfType<Jellyfish>();
        for (int i = 0; i < jellyfishes.Length; ++i)
        {
            jellyfishes[i].anim.SetTrigger("Wrong");
        }
        gameController.nextQuestion = true;
    }

    void realHitted()
    {
        gameController.updatePlayerPoint(true);
        gameController.updatePlayerQuestionData(true);
        if (correctAnswer != null)
            correctAnswer.text = choice;
        Jellyfish[] jellyfishes = FindObjectsOfType<Jellyfish>();
        for (int i = 0; i < jellyfishes.Length; ++i)
        {
            if (jellyfishes[i] == this)
                continue;
            jellyfishes[i].anim.SetTrigger("Correct");
        }
        gameController.nextQuestion = true;
        foreach (Player player in players)
        {
            if (player.isLocalPlayer)
            {
                player.CmdIncreaseScore();
                break;
            }
        }
    }

    void restoreInput()
    {
        Physics2DRaycaster physics2DRaycaster = mainCamera.GetComponent<Physics2DRaycaster>();
        if (physics2DRaycaster != null)
            physics2DRaycaster.enabled = true;
    }

    void spawnDisappearEffect()
    {
        Instantiate(disappearEffect, transform);
    }
}
