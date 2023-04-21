using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private float laneSwitchForce = 2.5f;
    [SerializeField] private Lane currentLane = Lane.Middle;
    [SerializeField] private Undulate currentUndulate = Undulate.Center;
    [Header("Lane Settings")]
    [SerializeField] private float laneXDistance = 4f;
    [SerializeField] private float laneYDistance = 3f;
    [SerializeField] private int lanes = 3;
    [Header("Lane Settings")]
    [SerializeField] private bool showGizmos = true;
    [Header("Variables")]
    private Vector3 pos;
    private Quaternion rotation;
    private Vector3 right;
    private Vector3 up;


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


    private void SwipeUp()
    {
        if (currentUndulate == Undulate.Center)
            currentUndulate = Undulate.Up;
        if (currentUndulate == Undulate.Down)
            currentUndulate = Undulate.Center;
    }

    private void SwipeDown()
    {
        if (currentUndulate == Undulate.Center)
            currentUndulate = Undulate.Down;
        if (currentUndulate == Undulate.Up)
            currentUndulate = Undulate.Center;
    }

    private void SwipeLeft()
    {
        if (currentLane == Lane.Middle)
            currentLane = Lane.Left;
        if (currentLane == Lane.Right)
            currentLane = Lane.Middle;
    }

    private void SwipeRight()
    {
        if (currentLane == Lane.Middle)
            currentLane = Lane.Right;
        if (currentLane == Lane.Left)
            currentLane = Lane.Middle;
    }


    private void Update()
    {
        pos = transform.position;
        rotation = transform.rotation;
        right = rotation * Vector3.right;
        up = rotation * Vector3.up;

        pos = startPosition + (laneXDistance * (float)currentLane) * right + (laneYDistance * (float)currentUndulate) * up;

        transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * laneSwitchForce);
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;
        Gizmos.color = Color.green;

        if (!Application.isPlaying)
        {
            rotation = transform.rotation;
            right = rotation * Vector3.right;
            up = rotation * Vector3.up;
        }

        for (int i = 0; i < lanes; i++)
        {
            for (int j = 0; j < lanes; j++)
            {
                Vector3 pos = startPosition + (-laneXDistance + laneXDistance * j) * right + (-laneYDistance + laneYDistance * i) * up;
                Gizmos.DrawSphere(pos, 0.2f);
            }
        }

    }

}

