using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphabetControl : MonoBehaviour
{
    Rigidbody2D rb2D;
    public float forceValue;
    WordBlockGameController wbGameController;
    // Start is called before the first frame update
    void Start()
    {
        wbGameController = GameObject.Find("WordBlockGameController").GetComponent<WordBlockGameController>();
        rb2D = GetComponent<Rigidbody2D>();
        if (wbGameController.questionsData[wbGameController.currentPlayingNum].content[1].Length <= 8)
            forceValue = -95f;
        else if (wbGameController.questionsData[wbGameController.currentPlayingNum].content[1].Length <= 13)
            forceValue = -115f;
        else 
            forceValue = -125f;
        rb2D.AddForce(new Vector2(forceValue, 0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
