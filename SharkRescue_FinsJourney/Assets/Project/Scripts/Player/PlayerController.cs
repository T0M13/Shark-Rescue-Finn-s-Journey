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

}

