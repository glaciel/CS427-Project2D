using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TrueOyster : MonoBehaviour
{
    PearlHunterGameController gameController;
    GameObject mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        gameController = FindObjectOfType<PearlHunterGameController>();
        mainCamera = GameObject.Find("Main Camera");
        Physics2DRaycaster physics2DRaycaster = mainCamera.GetComponent<Physics2DRaycaster>();
        if (physics2DRaycaster != null)
            physics2DRaycaster.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void beChoosen()
    {
        Animator fishAnim = gameController.fish.GetComponent<Animator>();
        fishAnim.SetTrigger("TrueAnswer");
        Physics2DRaycaster physics2DRaycaster = mainCamera.GetComponent<Physics2DRaycaster>();
        if (physics2DRaycaster != null)
            physics2DRaycaster.enabled = false;
    }

    void stopAll()
    {
        Physics2DRaycaster physics2DRaycaster = mainCamera.GetComponent<Physics2DRaycaster>();
        if (physics2DRaycaster != null)
            physics2DRaycaster.enabled = true;
        gameController.stopAll();
    }

    void checkAnswer()
    {
        if (gameController.questionsData[gameController.currentPlayingNum].content[gameController.curQuestion * 2 + 1] == "T")
            gameController.checkAnswer(true);
        else
            gameController.checkAnswer(false);
        Animator anim = gameController.falseOyster.GetComponent<Animator>();
        anim.SetTrigger("Answered");
    }
}
