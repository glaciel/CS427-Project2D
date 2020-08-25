using UnityEngine;
using UnityEngine.UI;

public class AddFindMatchMethod : MonoBehaviour
{
    private void OnEnable()
    {
        Prototype.NetworkLobby.MyLobbyManager lobbyManager = GameObject.Find("LobbyManager").GetComponent<Prototype.NetworkLobby.MyLobbyManager>();
        Button button = GetComponent<Button>();
        button.onClick.AddListener(lobbyManager.findMatch);
    }
}