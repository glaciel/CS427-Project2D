using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoginController : MonoBehaviour
{
    string nickname = "";
    string password = "";
    string studentID = "";
    string rePass = "";
    int rank;
    int rating;
    public GameObject InfoPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void getStudentID(string input)
    {
        studentID = input;
    }

    public void getNickname(string input)
    {
        nickname = input;
    }

    public void getPassword(string input)
    {
        password = input;
    }

    public void getRepass(string input)
    {
        rePass = input;
    }

    void ActiveInfoPanel(string input)
    {
        Text text = InfoPanel.GetComponentInChildren<Text>();
        text.text = input;
        InfoPanel.SetActive(true);
    }

    public void RegisterNewAccount()
    {
        if (studentID == "" || password == "" || nickname == "")
        {
            ActiveInfoPanel("Please fill in all fields");
        }
        else if (password != rePass)
        {
            ActiveInfoPanel("Your password not match");
        }
        else
            StartCoroutine(Register());
    }

    IEnumerator Register()
    {
        WWWForm form = new WWWForm();
        form.AddField("nickname", nickname);
        form.AddField("password", password);
        form.AddField("studentID", studentID);

        using (UnityWebRequest www = UnityWebRequest.Post("http://powerlishproject.dx.am/Register.php", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                ActiveInfoPanel(www.error);
            }
            else
            {
                ActiveInfoPanel(www.downloadHandler.text);
            }
        }
    }

    public void Login()
    {
        if (studentID == "" || password == "")
        {
            ActiveInfoPanel("Please fill in all fields");
        }
        else
            StartCoroutine(LoginCoroutine());
    }

    IEnumerator LoginCoroutine()
    {
        WWWForm form = new WWWForm();
        form.AddField("password", password);
        form.AddField("studentID", studentID);

        using (UnityWebRequest www = UnityWebRequest.Post("http://powerlishproject.dx.am/Login.php", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                ActiveInfoPanel(www.error);
            }
            else
            {
                if (www.downloadHandler.text == "login-SUCCESS")
                {
                    ActiveInfoPanel("Login successfully");
                    MainPlayer.instance.updateLoginData(studentID);
                    StartCoroutine(updateAppVersionCoroutine(studentID));
                }
                else
                    ActiveInfoPanel(www.downloadHandler.text);
            }
        }
    }

    IEnumerator updateAppVersionCoroutine(string studentID)
    {
        WWWForm form = new WWWForm();
        form.AddField("studentID", studentID);
        form.AddField("appVersion", Application.version);

        using (UnityWebRequest www = UnityWebRequest.Post("http://powerlishproject.dx.am/updateAppVersion.php", form))
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
