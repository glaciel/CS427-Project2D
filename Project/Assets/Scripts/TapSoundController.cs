using UnityEngine;

public class TapSoundController : MonoBehaviour
{
    AudioSource tapSound;
    // Start is called before the first frame update
    void Start()
    {
        tapSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Ended)
            {
                tapSound.Play();   
            }
        }
    }
}
