using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData : MonoBehaviour
{
    [System.Serializable]
    public class Ratio
    {
        public float value;
        public int correct;
        public int meet;

        public Ratio()
        {
            value = 0;
            correct = 0;
            meet = 0;
        }

        public void updateValue()
        {
            if (meet == 0)
                return;
            value = (float)correct / meet;
        }
    }

    [System.Serializable]
    public class PlayerDataDictionary : SerializableDictionary<int, Ratio> { }
    public string studentID;
    public string nickname;
    public int rank;
    public int rating;
    public Ratio listeningPoint;
    public Ratio vocabularyPoint;
    public Ratio grammarPoint;
    public List<PlayerDataDictionary> metVocabulary = new List<PlayerDataDictionary>();
    public List<PlayerDataDictionary> metListening = new List<PlayerDataDictionary>();
    public List<PlayerDataDictionary> metGrammar = new List<PlayerDataDictionary>();
    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateMetQuestionsData(List<PlayerDataDictionary> target, Question input, bool correct)
    {
        Ratio value;
        while (target.Count < input.unit)
            target.Add(new PlayerDataDictionary());
        //Check if the question have been met before
        if (!target[input.unit - 1].ContainsKey(input.ID))
        {
            //If not, add this question to dict
            target[input.unit - 1].Add(input.ID, new Ratio());
        }
        //Update the ratio
        target[input.unit - 1].TryGetValue(input.ID, out value);
        ++value.meet;
        if (correct)
            ++value.correct;
        value.updateValue();
    }
}
