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
                if(currentLane == middleLaneTransform)
                {
                    ChangeLane(leftLaneTransform);
                }
                if (currentLane == rightLaneTransform)
                {
                    ChangeLane(middleLaneTransform);
                }
                break;
            case SwipeType.Right:
                if (currentLane == middleLaneTransform)
                {
                    ChangeLane(rightLaneTransform);
                }
                if (currentLane == leftLaneTransform)
                {
                    ChangeLane(middleLaneTransform);
                }
                break;
        }
    }

    private void ChangeLane(Transform lane)
    {
        if (lane == null) return;
        transform.position = lane.position;
        currentLane = lane;
    }


}
