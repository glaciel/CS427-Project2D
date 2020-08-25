using UnityEngine;

public class TapToPlay : MonoBehaviour
{
    public GameObject loginPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            loginPanel.SetActive(true);
            enabled = false;
        }
    }
}