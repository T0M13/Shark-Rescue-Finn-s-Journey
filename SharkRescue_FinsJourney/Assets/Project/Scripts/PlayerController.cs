using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Lanes")]
    [SerializeField] private Transform leftLaneTransform;
    [SerializeField] private Transform middleLaneTransform;
    [SerializeField] private Transform rightLaneTransform;
    [Header("Settings")]
    [SerializeField] private Transform currentLane;
    [SerializeField] private float elapsedTime = 0;
    [SerializeField] private float waitTime = 3f;
    [SerializeField] private Coroutine switchCoroutine;

    private void Awake()
    {
        currentLane = middleLaneTransform;
    }

    private void OnEnable()
    {
        InputManager.instance.swipeDetector.OnSwipeUp += SwipeUp;
        InputManager.instance.swipeDetector.OnSwipeDown += SwipeDown;
        InputManager.instance.swipeDetector.OnSwipeLeft += SwipeLeft;
        InputManager.instance.swipeDetector.OnSwipeRight += SwipeRight;
    }

    private void OnDisable()
    {
        InputManager.instance.swipeDetector.OnSwipeUp -= SwipeUp;
        InputManager.instance.swipeDetector.OnSwipeDown -= SwipeDown;
        InputManager.instance.swipeDetector.OnSwipeLeft -= SwipeLeft;
        InputManager.instance.swipeDetector.OnSwipeRight -= SwipeRight;
    }


    private void SwipeUp(SwipeType swipeType)
    {
        MoveLane(swipeType);
    }

    private void SwipeDown(SwipeType swipeType)
    {
        MoveLane(swipeType);
    }

    private void SwipeLeft(SwipeType swipeType)
    {
        MoveLane(swipeType);
    }

    private void SwipeRight(SwipeType swipeType)
    {
        MoveLane(swipeType);
    }

    private void MoveLane(SwipeType swipeType)
    {
        switch (swipeType)
        {
            case SwipeType.Left:
                if (currentLane == middleLaneTransform)
                {
                    if (switchCoroutine != null)
                        break;
                    switchCoroutine = StartCoroutine(ChangeLane(leftLaneTransform));
                }
                if (currentLane == rightLaneTransform)
                {
                    if (switchCoroutine != null)
                        break;
                    switchCoroutine = StartCoroutine(ChangeLane(middleLaneTransform));
                }
                break;
            case SwipeType.Right:
                if (currentLane == middleLaneTransform)
                {
                    if (switchCoroutine != null)
                        break;
                    switchCoroutine = StartCoroutine(ChangeLane(rightLaneTransform));
                }
                if (currentLane == leftLaneTransform)
                {
                    if (switchCoroutine != null)
                        break;
                    switchCoroutine = StartCoroutine(ChangeLane(middleLaneTransform));
                }
                break;
        }
    }

    private IEnumerator ChangeLane(Transform lane)
    {
        while (elapsedTime < 1)
        {
            elapsedTime += Time.deltaTime / waitTime;

            if (elapsedTime > 1) elapsedTime = 1;

            transform.position = Vector3.Lerp(transform.position, lane.position, elapsedTime);
            yield return null;

        }

        transform.position = lane.position;
        currentLane = lane;
        elapsedTime = 0;
        switchCoroutine = null;

        yield return null;

    }


}
