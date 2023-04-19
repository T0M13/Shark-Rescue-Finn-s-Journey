using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Lanes")]
    [SerializeField] private Transform leftLaneTransform;
    [SerializeField] private Transform middleLaneTransform;
    [SerializeField] private Transform rightLaneTransform;
    [Header("Lane Switch Settings")]
    [SerializeField] private Transform currentLane;
    [SerializeField] private float laneElapsedTime = 0;
    [SerializeField] private float laneWaitTime = 0.1f;
    [SerializeField] private float laneSwitchSpeed = 0.5f;
    [SerializeField] private Coroutine laneSwitchCoroutine;
    [Header("Player Settings")]
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private float force = 2.5f;
    [Header("Swim Settings")]
    [SerializeField] private Coroutine swimCoroutine;
    [SerializeField] private AnimationCurve swimCurve;
    [SerializeField] private float swimWaitTime = 1f;
    [SerializeField] private float swimReturnWaitTime = 1f;
    [SerializeField] private float swimReturnElapsedTime = 0;
    [SerializeField] private float swimElapsedTime = 0;



    private void Awake()
    {
        currentLane = middleLaneTransform;
        startPosition = transform.position;
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
        if (swimCoroutine != null)
            return;
        swimCoroutine = StartCoroutine(SwimInDirection(Vector3.up));
    }

    private void SwipeDown(SwipeType swipeType)
    {
        if (swimCoroutine != null)
            return;
        swimCoroutine = StartCoroutine(SwimInDirection(Vector3.down));
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
                    if (laneSwitchCoroutine != null)
                        break;
                    laneSwitchCoroutine = StartCoroutine(ChangeLane(leftLaneTransform));
                }
                if (currentLane == rightLaneTransform)
                {
                    if (laneSwitchCoroutine != null)
                        break;
                    laneSwitchCoroutine = StartCoroutine(ChangeLane(middleLaneTransform));
                }
                break;
            case SwipeType.Right:
                if (currentLane == middleLaneTransform)
                {
                    if (laneSwitchCoroutine != null)
                        break;
                    laneSwitchCoroutine = StartCoroutine(ChangeLane(rightLaneTransform));
                }
                if (currentLane == leftLaneTransform)
                {
                    if (laneSwitchCoroutine != null)
                        break;
                    laneSwitchCoroutine = StartCoroutine(ChangeLane(middleLaneTransform));
                }
                break;
        }
    }

    private IEnumerator SwimInDirection(Vector3 direction)
    {
        Vector3 newPosition = new Vector3(transform.position.x, startPosition.y, transform.position.z);

        swimElapsedTime = 0;
        while (swimElapsedTime < swimWaitTime)
        {
            swimElapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.localPosition, newPosition + direction * force, swimCurve.Evaluate(swimElapsedTime));
            yield return null;
        }

        StartCoroutine(SwimBackCenter());
    }

    private IEnumerator SwimBackCenter()
    {
        Vector3 newPosition = new Vector3(transform.position.x, startPosition.y, transform.position.z);

        swimReturnElapsedTime = 0;
        while (swimReturnElapsedTime < swimReturnWaitTime)
        {
            swimReturnElapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.localPosition, newPosition, swimCurve.Evaluate(swimReturnElapsedTime));
            yield return null;
        }

        transform.position = newPosition;
        swimCoroutine = null;
    }

    private IEnumerator ChangeLane(Transform lane)
    {
        float t = 0;
        Vector3 newPosition = new Vector3(lane.position.x, transform.position.y, transform.position.z);
        while (laneElapsedTime < 1)
        {
            laneElapsedTime += (Time.deltaTime * laneSwitchSpeed) / laneWaitTime;
            t += Time.deltaTime;

            if (laneElapsedTime > 1) laneElapsedTime = 1;

            transform.position = Vector3.Lerp(transform.position, newPosition, swimCurve.Evaluate(t));
            yield return null;

        }

        transform.position = newPosition;
        currentLane = lane;
        laneElapsedTime = 0;
        laneSwitchCoroutine = null;

        yield return null;

    }


}
