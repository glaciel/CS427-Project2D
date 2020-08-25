using UnityEngine;

public class SwipeController : MonoBehaviour
{
    public GameObject[] gameModes;
    private Vector2 fingerDownPosition;
    private Vector2 fingerUpPosition;
    private int currentMode = 0;

    private float minDistanceForSwipe = 20f;

    private void Start()
    {

    }
    private void Update()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerUpPosition = touch.position;
                fingerDownPosition = touch.position;
            }

            if (touch.phase == TouchPhase.Ended)
            {
                fingerDownPosition = touch.position;
                DetectSwipe();
            }
        }
    }

    private void DetectSwipe()
    {
        if (SwipeDistanceCheckMet())
        {
            if (IsVerticalSwipe())
            {

            }
            else
            {
                var direction = fingerDownPosition.x - fingerUpPosition.x > 0 ? SwipeDirection.Right : SwipeDirection.Left;
                changeMode(direction);
            }
            fingerUpPosition = fingerDownPosition;
        }
    }

    private bool IsVerticalSwipe()
    {
        return VerticalMovementDistance() > HorizontalMovementDistance();
    }

    private bool SwipeDistanceCheckMet()
    {
        return VerticalMovementDistance() > minDistanceForSwipe || HorizontalMovementDistance() > minDistanceForSwipe;
    }

    private float VerticalMovementDistance()
    {
        return Mathf.Abs(fingerDownPosition.y - fingerUpPosition.y);
    }

    private float HorizontalMovementDistance()
    {
        return Mathf.Abs(fingerDownPosition.x - fingerUpPosition.x);
    }

    void changeMode(SwipeDirection dir)
    {
        if (dir == SwipeDirection.Left)
        {
            Animator anim = gameModes[currentMode].GetComponent<Animator>();
            anim.SetTrigger("MoveOutLeft");
            currentMode = (currentMode + 1) % gameModes.Length;
            gameModes[currentMode].SetActive(true);
            anim = gameModes[currentMode].GetComponent<Animator>();
            anim.SetTrigger("MoveInLeft");
        }
        else
        {
            Animator anim = gameModes[currentMode].GetComponent<Animator>();
            anim.SetTrigger("MoveOutRight");
            --currentMode;
            if (currentMode == -1)
                currentMode = gameModes.Length - 1;
            gameModes[currentMode].SetActive(true);
            anim = gameModes[currentMode].GetComponent<Animator>();
            anim.SetTrigger("MoveInRight");
        }
    }
}

public enum SwipeDirection
{
    Up,
    Down,
    Left,
    Right
}
