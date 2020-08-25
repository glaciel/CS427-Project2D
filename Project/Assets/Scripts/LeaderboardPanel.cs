using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LeaderboardPanel : MonoBehaviour
{
    public GameObject top10;
    public Sprite[] fishSprites;
    public GameObject infoPrefab;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void showLeaderboard()
    {
        gameObject.SetActive(true);
        StartCoroutine(getTop10DataCoroutine());
    }

    IEnumerator getTop10DataCoroutine()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("http://powerlishproject.dx.am/getTop10.php"))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string[] resultLines = www.downloadHandler.text.Split('\n');
                //du 1 line do ky tu \n cuoi cung
                for (int i = 0; i < resultLines.Length - 1; ++i)
                {
                    string[] result = resultLines[i].Split('/');
                    GameObject cur = Instantiate(infoPrefab, top10.transform);
                    if (i % 2 == 0)
                    {
                        Image img = cur.GetComponentInChildren<Image>();
                        img.color = Color.white;
                    }
                    Text nickname = cur.transform.Find("Nickname").GetComponent<Text>();
                    nickname.text = result[0];
                    Image fish = cur.transform.Find("Fish").GetComponent<Image>();
                    switch(result[1])
                    {
                        case "Nemo":
                            fish.sprite = fishSprites[0];
                            break;
                        case "Angler":
                            fish.sprite = fishSprites[1];
                            break;
                        case "Sword":
                            fish.sprite = fishSprites[2];
                            break;
                        case "Shark":
                            fish.sprite = fishSprites[3];
                            break;
                        default:
                            break;
                    }
                    Text rating = cur.transform.Find("Rating").GetComponent<Text>();
                    rating.text = result[2];
                }
            }
        }
    }

    public void hideLeaderboard()
    {
        Transform transform = top10.GetComponent<Transform>();
        for (int i = 0; i < transform.childCount; ++i)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        gameObject.SetActive(false);
    }
}
