using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishPearlHunter : MonoBehaviour
{
    PearlHunterGameController gameController;
    // Start is called before the first frame update
    void Start()
    {
        gameController = FindObjectOfType<PearlHunterGameController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void chooseTrueAnswer()
    {
        Animator oysterAnim = gameController.trueOyster.GetComponent<Animator>();
        oysterAnim.SetTrigger("Open");
    }

    void chooseFalseAnswer()
    {
        Animator oysterAnim = gameController.falseOyster.GetComponent<Animator>();
        oysterAnim.SetTrigger("Open");
    }
}
