using UnityEngine;
using UnityEngine.UI;

public class ArenaInfo : MonoBehaviour
{
    public Text nickname;
    public Text rating;
    public Text rank;
    // Start is called before the first frame update
    void Start()
    {
        MainPlayer.instance.updateRankData();
        UpdateInfo();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateInfo()
    {
        nickname.text = MainPlayer.instance.playerData.nickname;
        rank.text = "Rank: " + MainPlayer.instance.playerData.rank.ToString();
        rating.text = MainPlayer.instance.playerData.rating.ToString();
    }
}
