using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.IO;

public class MainPlayer : MonoBehaviour
{
    public static MainPlayer instance;
    public PlayerData playerData;
    public string fishName;
    float startAppTime;
    float quitAppTime;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        playerData = GetComponent<PlayerData>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator getDataCoroutine(string studentID)
    {
        WWWForm form = new WWWForm();
        form.AddField("studentID", studentID);

        using (UnityWebRequest www = UnityWebRequest.Post("http://powerlishproject.dx.am/getBasicData.php", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string[] result = www.downloadHandler.text.Split('/');
                playerData.nickname = result[0];
                playerData.rank = int.Parse(result[1]);
                playerData.rating = int.Parse(result[2]);
                playerData.studentID = studentID;
            }
        }
    }

    public void updateRankData()
    {
        StartCoroutine(updateRankDataCoroutine());
    }

    IEnumerator updateRankDataCoroutine()
    {
        WWWForm form = new WWWForm();
        form.AddField("studentID", playerData.studentID);

        using (UnityWebRequest www = UnityWebRequest.Post("http://powerlishproject.dx.am/updateRank.php", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string result = www.downloadHandler.text;
                playerData.rank = int.Parse(result);
            }
        }
    }

    public void updateLoginData(string studentID)
    {
        startAppTime = Time.realtimeSinceStartup;
        StartCoroutine(getFishNameCoroutine(studentID));
        StartCoroutine(loadPlayerDataCoroutine(studentID));
    }

    IEnumerator loadPlayerDataCoroutine(string studentID)
    {
        WWWForm form = new WWWForm();
        form.AddField("studentID", studentID);
        using (UnityWebRequest www = UnityWebRequest.Post("http://powerlishproject.dx.am/loadPlayerData.php", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string dataAsJson = www.downloadHandler.text;
                if (dataAsJson != "null") 
                    JsonUtility.FromJsonOverwrite(dataAsJson, playerData);
                else
                    StartCoroutine(getDataCoroutine(studentID));
                SceneManager.LoadSceneAsync(1);
            }
        }
    }

    //Up playerData to store in server
    public void updatePlayerData()
    {
        StartCoroutine(updateRatingDataCoroutine());
        StartCoroutine(updatePlayerDataCoroutine());
    }

    IEnumerator updatePlayerDataCoroutine()
    {
        string dataAsJson = JsonUtility.ToJson(playerData);
        using (UnityWebRequest www = UnityWebRequest.Put("http://powerlishproject.dx.am/uploadPlayerData.php", dataAsJson))
        {
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {

            }
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            updatePlayerData();
        }
    }
    private void OnApplicationQuit()
    {
        updatePlayerData();
        quitAppTime = Time.realtimeSinceStartup;
        StartCoroutine(updateUsingAppTimeCoroutine());
    }

    IEnumerator updateRatingDataCoroutine()
    {
        WWWForm form = new WWWForm();
        form.AddField("studentID", playerData.studentID);
        form.AddField("rating", playerData.rating);

        using (UnityWebRequest www = UnityWebRequest.Post("http://powerlishproject.dx.am/updateRating.php", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                //Debug.Log(www.downloadHandler.text);
            }
        }
    }

    IEnumerator getFishNameCoroutine(string studentID)
    {
        WWWForm form = new WWWForm();
        form.AddField("studentID", studentID);

        using (UnityWebRequest www = UnityWebRequest.Post("http://powerlishproject.dx.am/getFishName.php", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                fishName = www.downloadHandler.text;
            }
        }
    }

    IEnumerator updateUsingAppTimeCoroutine()
    {
        WWWForm form = new WWWForm();
        form.AddField("studentID", playerData.studentID);
        form.AddField("usingTime", (int)(quitAppTime - startAppTime));

        using (UnityWebRequest www = UnityWebRequest.Post("http://powerlishproject.dx.am/updateUsingAppTime.php", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                //Debug.Log(www.downloadHandler.text);
            }
        }
    }
}
