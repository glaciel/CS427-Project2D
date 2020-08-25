using UnityEngine;

using Prototype.NetworkLobby;

public class FishVSContainer : MonoBehaviour
{
    public GameObject[] fishPrefabs;
    // Start is called before the first frame update
    void Start()
    {
        //Spawn fish VS Icon in-game
        if (MyLobbyManager.s_Singleton != null)
        {
            for (int i = 0; i < MyLobbyManager.s_Singleton.lobbySlots.Length; ++i)
            {
                MyLobbyPlayer cur = MyLobbyManager.s_Singleton.lobbySlots[i] as MyLobbyPlayer;
                if (cur != null) 
                    if (cur.rank != ("Rank: " + MainPlayer.instance.playerData.rank.ToString()))
                        instantiateFish(cur.fishName);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void instantiateFish(string fishName)
    {
        foreach (GameObject go in fishPrefabs)
        {
            if (go.name == (fishName + "VSIcon"))
            {
                Instantiate(go, transform);
                break;
            }
        }
    }
}
