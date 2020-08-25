using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Question
{
    public int ID;
    public int unit;
    public string[] content;

    public virtual void getQuestionData(string line, int unit)
    {

    }
}
