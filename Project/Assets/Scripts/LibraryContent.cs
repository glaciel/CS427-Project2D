using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LibraryContent : MonoBehaviour
{
    public TextAsset[] vocab;
    public GameObject wordPrefab;
    public GameObject grammarPrefab;
    public GameObject wordList;
    public GameObject grammarList;
    public GameObject vocabulary;
    public int currentUnit;
    List<string> vocabList;
    // Start is called before the first frame update
    void Start()
    {
        vocabList = new List<string>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void getVocabList(int index)
    {
        List<string> data = new List<string>();
        data.AddRange(System.Text.RegularExpressions.Regex.Split(vocab[index].text, "\n|\r\n"));
        data.RemoveAt(0);
        foreach (string line in data)
        {
            string[] lineContent = System.Text.RegularExpressions.Regex.Split(line, ",");
            vocabList.Add(lineContent[1]);
        }
    }

    public void showVocabList()
    {
        getVocabList(currentUnit);
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, rectTransform.rect.height + 100 * vocabList.Count);
        rectTransform = vocabulary.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, rectTransform.rect.height + 100 * vocabList.Count);
        for (int i = 0; i < vocabList.Count; ++i)
        {
            GameObject cur = Instantiate(wordPrefab, wordList.transform);
            if (i % 2 == 0)
            {
                Image img = cur.GetComponentInChildren<Image>();
                img.color = Color.white;
            }
            Text text = cur.GetComponentInChildren<Text>();
            text.text = vocabList[i];
        }
    }

    public void setCurUnit(int unit)
    {
        currentUnit = unit;
    }

    public void hideVocabList()
    {
        if (wordList.transform.childCount != 0)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, rectTransform.rect.height - 100 * vocabList.Count);
            rectTransform = vocabulary.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, rectTransform.rect.height - 100 * vocabList.Count);
        }
        Transform transform = wordList.GetComponent<Transform>();
        for(int i = 0; i < transform.childCount; ++i)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        vocabList.Clear();
    }

    public void showGrammarList()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, rectTransform.rect.height + 100);
        GameObject cur = Instantiate(grammarPrefab, grammarList.transform);
        Button button = cur.GetComponentInChildren<Button>();
        button.onClick.AddListener(openGrammarLink);
    }

    public void hideGrammarList()
    {
        if (grammarList.transform.childCount != 0)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, rectTransform.rect.height - 100);
        }
        Transform transform = grammarList.GetComponent<Transform>();
        for (int i = 0; i < transform.childCount; ++i)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    public void openGrammarLink()
    {
        switch (currentUnit)
        {
            case 0:
                Application.OpenURL("https://drive.google.com/open?id=1Ydu2OKSPSZqFmSGy2XS_Gs4-Nq8NVTqE");
                break;
            case 1:
                Application.OpenURL("https://drive.google.com/open?id=1CuatSsPkq1qgefW3a4PLjdpc29Ex9nje");
                break;
            case 2:
                Application.OpenURL("https://drive.google.com/open?id=1jHUI68q0U2bKK-OToTaQLndX0hjQhEsq");
                break;
            case 3:
                Application.OpenURL("https://drive.google.com/open?id=18vb5Nk0aqX5PFHWTdQ88M3y4qImS277X");
                break;
            case 4:
                Application.OpenURL("https://drive.google.com/open?id=1JwSf7Oc4AGYMHMwtoQGK9BD6HQvc3kAn");
                break;
            case 5:
                Application.OpenURL("https://drive.google.com/open?id=1cIQYW0s5__O31fJkGc7NRAlVebAcxsOc");
                break;
            case 6:
                Application.OpenURL("https://drive.google.com/open?id=1RoHokikZKj54ILtv0dAkkjVqeMZHFnN0");
                break;
            case 7:
                Application.OpenURL("https://drive.google.com/open?id=1ui9__TWR_yYPTENr6M5SK4iz8udqA1ZB");
                break;
            default:
                break;
        }
    }
}
