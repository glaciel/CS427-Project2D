using UnityEngine;

public class FishContainer : MonoBehaviour
{
    public GameObject[] fishPrefabs;
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject go in fishPrefabs)
        {
            if (go.name == (MainPlayer.instance.fishName + "Icon"))
            {
                Instantiate(go, transform);
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
