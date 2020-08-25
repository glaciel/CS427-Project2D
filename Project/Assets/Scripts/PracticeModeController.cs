using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class PracticeModeController : MonoBehaviour
{
    public MiniGameController gameController;
    public Slider process;
    public GameObject howToPlayPanel;
    public GameObject result;
    bool endPractice = false;
    GameObject canvas;
    GameObject mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.Find("Main Camera");
        canvas = GameObject.Find("Canvas");
        StartCoroutine(countdownStartTime());
    }

    // Update is called once per frame
    void Update()
    {
        Text text = process.GetComponentInChildren<Text>();
        text.text = gameController.totalQuestion + "/10";
        process.value = gameController.totalQuestion;
        if (gameController.totalQuestion == 10)
        {
            if (!endPractice)
            {
                endPracticeFunc();
                endPractice = true;
            }
            return;
        }

    }

    public void BackToMainMenu()
    {
        Resources.UnloadUnusedAssets();
        SceneManager.LoadSceneAsync("Arena");
    }

    IEnumerator countdownStartTime()
    {
        yield return new WaitForSecondsRealtime(4.0f);
        howToPlayPanel.SetActive(false);
    }

    void endPracticeFunc()
    {
        StartCoroutine(updatePracticeTimesCoroutine());
        Animator anim = result.GetComponent<Animator>();
        anim.SetTrigger("Finish");
        Invoke("BackToMainMenu", 3.5f);
        GraphicRaycaster graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
        graphicRaycaster.enabled = false;
        Physics2DRaycaster physics2DRaycaster = mainCamera.GetComponent<Physics2DRaycaster>();
        if (physics2DRaycaster != null)
            physics2DRaycaster.enabled = false;
    }

    IEnumerator updatePracticeTimesCoroutine()
    {
        WWWForm form = new WWWForm();
        form.AddField("studentID", MainPlayer.instance.playerData.studentID);

        using (UnityWebRequest www = UnityWebRequest.Post("http://powerlishproject.dx.am/updatePracticeTimes.php", form))
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
