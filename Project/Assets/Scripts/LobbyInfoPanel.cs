using UnityEngine;
using UnityEngine.UI;


namespace Prototype.NetworkLobby 
{
    public class LobbyInfoPanel : MonoBehaviour
    {
        public Button cancelButton;

        void Start()
        {
            
        }

        public void addBackFunction()
        {
            cancelButton.onClick.AddListener(() => { MyLobbyManager.s_Singleton.backDelegate(); });
        }

        public void display()
        {
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(() => { gameObject.SetActive(false); });
            gameObject.SetActive(true);
        }
    }
}