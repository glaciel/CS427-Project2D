using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameController : MonoBehaviour
{
    public int totalQuestion = 0; //used to determine played question in practice mode
    public bool practiceMode;
    public bool startGame;
    public int practiceUnit;
    public virtual void updatePlayerPoint(bool correct)
    {

    }

    public virtual void updatePlayerQuestionData(bool correct)
    {

    }

    //Random function with different probabilities
    protected int Choose(float[] probs)
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

    public void startGamePractice()
    {
        startGame = true;
    }

    public int pickRandUnit(int unitCount)
    {
        switch(unitCount)
        {
            case 3:
                float[] probs3 = { 0.1f, 0.1f, 0.8f };
                return Choose(probs3);
            case 4:
                float[] probs4 = { 0.05f, 0.05f, 0.2f, 0.7f };
                return Choose(probs4);
            case 6:
                float[] probs6 = { 0.05f, 0.05f, 0.05f, 0.05f, 0.4f, 0.4f };
                return Choose(probs6);
            case 8:
                float[] probs8 = { 0.025f, 0.025f, 0.05f, 0.05f, 0.1f, 0.1f, 0.325f, 0.325f };
                return Choose(probs8);
            default:
                return 0;
        }
    }

    public void pickPracticeUnit(int unit)
    {
        practiceUnit = unit;
    }
}
