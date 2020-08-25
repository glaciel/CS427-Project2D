using UnityEngine;
using UnityEngine.UI;

public class Volcano : MonoBehaviour
{
    WordBlockGameController wbGameController;
    // Start is called before the first frame update
    void Start()
    {
        wbGameController = GameObject.Find("WordBlockGameController").GetComponent<WordBlockGameController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void stopAll()
    {
        wbGameController.stopAll();
        GraphicRaycaster graphicRaycaster = GetComponent<GraphicRaycaster>();
        graphicRaycaster.enabled = true;
    }

    void disableInteractive()
    {
        GraphicRaycaster graphicRaycaster = GetComponent<GraphicRaycaster>();
        graphicRaycaster.enabled = false;
    }
}
