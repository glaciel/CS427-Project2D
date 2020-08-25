using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PracticePlayButton : MonoBehaviour
{
    string gameScene;
    // Start is called before the first frame update
    void Start()
    {
        gameScene = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeGameSceneName()
    {
        gameScene = EventSystem.current.currentSelectedGameObject.name;
    }

    public void changeGameScene()
    {
        if (gameScene != "")
            SceneManager.LoadSceneAsync(gameScene);
    }
}
