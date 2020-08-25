using UnityEngine;

public class RockLightController : MonoBehaviour
{
    public GameObject[] rockLights;
    public GameObject current;
    int currentIndex;
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        currentIndex = Random.Range(0, rockLights.Length);
        current = rockLights[currentIndex];
        current.SetActive(true);
        anim = current.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 > 0.95f)
        {
            current.SetActive(false);
            currentIndex = Random.Range(0, rockLights.Length);
            current = rockLights[currentIndex];
            current.SetActive(true);
            anim = current.GetComponent<Animator>();
        }
    }
}
