using UnityEngine;

public class Fish : MonoBehaviour
{
    public GameObject bubbleBeamPrefab;
    BoxCollider2D boxCollider;
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void shotBubbleBeam()
    {
        Instantiate(bubbleBeamPrefab, transform);
    }

    void removeCollider()
    {
        boxCollider.enabled = false;
    }

    void resumeCollider()
    {
        boxCollider.enabled = true;
    }
}
