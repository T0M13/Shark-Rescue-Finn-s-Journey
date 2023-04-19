using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private float sideForce = 2.5f;
    [SerializeField] private float uprightForce = 3f;
    [SerializeField] private Lane currentLane = Lane.Middle;
    [SerializeField] private Undulate currentUndulate = Undulate.Center;
    [Header("Lane Settings")]
    [SerializeField] private float laneXDistance = 4f;
    [SerializeField] private float laneYDistance = 3f;
    [SerializeField] private int lanes = 3;
    [Header("Lane Settings")]
    [SerializeField] private bool showGizmos = true;

    //[Header("Lane Switch Settings")]
    //[SerializeField] private float laneElapsedTime = 0;
    //[SerializeField] private float laneWaitTime = 0.1f;
    //[SerializeField] private float laneSwitchSpeed = 0.5f;
    //[SerializeField] private Coroutine laneSwitchCoroutine;
    //[Header("Swim Settings")]
    //[SerializeField] private Coroutine swimCoroutine;
    //[SerializeField] private AnimationCurve swimCurve;
    //[SerializeField] private float swimWaitTime = 1f;
    //[SerializeField] private float swimReturnWaitTime = 1f;
    //[SerializeField] private float swimReturnElapsedTime = 0;
    //[SerializeField] private float swimElapsedTime = 0;



    private void Awake()
    {
        startPosition = transform.position;
        currentLane = Lane.Middle;
        currentUndulate = Undulate.Center;
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
        if (currentUndulate == Undulate.Center)
            currentUndulate = Undulate.Up;
        if (currentUndulate == Undulate.Down)
            currentUndulate = Undulate.Center;
    }

    private void SwipeDown(SwipeType swipeType)
    {
        if (currentUndulate == Undulate.Center)
            currentUndulate = Undulate.Down;
        if (currentUndulate == Undulate.Up)
            currentUndulate = Undulate.Center;
    }

    private void SwipeLeft(SwipeType swipeType)
    {
        if (currentLane == Lane.Middle)
            currentLane = Lane.Left;
        if (currentLane == Lane.Right)
            currentLane = Lane.Middle;
    }

    private void SwipeRight(SwipeType swipeType)
    {
        if (currentLane == Lane.Middle)
            currentLane = Lane.Right;
        if (currentLane == Lane.Left)
            currentLane = Lane.Middle;
    }

    private void Update()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Lerp(pos.x, startPosition.x + laneXDistance * (float)currentLane, Time.deltaTime * sideForce);
        pos.y = Mathf.Lerp(pos.y, startPosition.y + laneYDistance * (float)currentUndulate, Time.deltaTime * uprightForce);
        transform.position = pos;
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;
        Gizmos.color = Color.green;

        for (int i = 0; i < lanes; i++)
        {
            for (int j = 0; j < lanes; j++)
            {
                Gizmos.DrawSphere(new Vector3(startPosition.x + (-laneXDistance + laneXDistance * j), startPosition.y + (-laneYDistance + laneYDistance * i), transform.position.z), 0.2f);
            }
        }
    }


    //private void MoveLane(SwipeType swipeType)
    //{
    //    switch (swipeType)
    //    {
    //        case SwipeType.Left:
    //            if (currentLane == middleLaneTransform)
    //            {
    //                if (laneSwitchCoroutine != null)
    //                    break;
    //                laneSwitchCoroutine = StartCoroutine(ChangeLane(leftLaneTransform));
    //            }
    //            if (currentLane == rightLaneTransform)
    //            {
    //                if (laneSwitchCoroutine != null)
    //                    break;
    //                laneSwitchCoroutine = StartCoroutine(ChangeLane(middleLaneTransform));
    //            }
    //            break;
    //        case SwipeType.Right:
    //            if (currentLane == middleLaneTransform)
    //            {
    //                if (laneSwitchCoroutine != null)
    //                    break;
    //                laneSwitchCoroutine = StartCoroutine(ChangeLane(rightLaneTransform));
    //            }
    //            if (currentLane == leftLaneTransform)
    //            {
    //                if (laneSwitchCoroutine != null)
    //                    break;
    //                laneSwitchCoroutine = StartCoroutine(ChangeLane(middleLaneTransform));
    //            }
    //            break;
    //    }
    //}

    //private IEnumerator SwimInDirection(Vector3 direction)
    //{
    //    Vector3 newPosition = new Vector3(transform.position.x, startPosition.y, transform.position.z);

    //    swimElapsedTime = 0;
    //    while (swimElapsedTime < swimWaitTime)
    //    {
    //        swimElapsedTime += Time.deltaTime;
    //        transform.position = Vector3.Lerp(transform.localPosition, newPosition + direction * force, swimCurve.Evaluate(swimElapsedTime));
    //        yield return null;
    //    }

    //    StartCoroutine(SwimBackCenter());
    //}

    //private IEnumerator SwimBackCenter()
    //{
    //    Vector3 newPosition = new Vector3(transform.position.x, startPosition.y, transform.position.z);

    //    swimReturnElapsedTime = 0;
    //    while (swimReturnElapsedTime < swimReturnWaitTime)
    //    {
    //        swimReturnElapsedTime += Time.deltaTime;
    //        transform.position = Vector3.Lerp(transform.localPosition, newPosition, swimCurve.Evaluate(swimReturnElapsedTime));
    //        yield return null;
    //    }

    //    transform.position = newPosition;
    //    swimCoroutine = null;
    //}

    //private IEnumerator ChangeLane(Transform lane)
    //{
    //    float t = 0;
    //    Vector3 newPosition = new Vector3(lane.position.x, transform.position.y, transform.position.z);
    //    while (laneElapsedTime < 1)
    //    {
    //        laneElapsedTime += (Time.deltaTime * laneSwitchSpeed) / laneWaitTime;
    //        t += Time.deltaTime;

    //        if (laneElapsedTime > 1) laneElapsedTime = 1;

    //        transform.position = Vector3.Lerp(transform.position, newPosition, swimCurve.Evaluate(t));
    //        yield return null;

    //    }

    //    transform.position = newPosition;
    //    currentLane = lane;
    //    laneElapsedTime = 0;
    //    laneSwitchCoroutine = null;

    //    yield return null;

    //}


}


public enum Lane
{
    Left = -1,
    Middle = 0,
    Right = 1
}

public enum Undulate
{
    Up = 1,
    Center = 0,
    Down = -1
}